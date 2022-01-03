using Newtonsoft.Json.Linq;
using App.Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Common
{
    public class BaseRequest
    {
        public readonly Database Db = Database.GetDefault();

        public Dictionary<string, object> Context = new Dictionary<string, object>();

        public void Require(params string[] fields)
        {
            if (fields == null || fields.Length <= 0)
            {
                return;
            }

            var field_list = fields.ToList();
            field_list.ForEach(field =>
            {
                var property = this.GetType().GetProperty(field);
                if (property == null || property.GetValue(this) == null)
                {
                    throw new Exception($"this api must have {field} field");
                }
            });
        }
    }

    public static class BaseRequestExtension
    {
        public static void From<T>(this T data, JObject param)
        {
            Convert(data, data.GetType(), param);
        }

        private static T Convert<T>(T data, Type type, JObject param)
        {
            var properties = type.GetProperties();

            if (properties == null || !properties.Any())
            {
                return data;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var compare_str = property.Name.ToCamelcase();
                var attribute = (FieldAttribute)Attribute.GetCustomAttribute(property, typeof(FieldAttribute));
                if (attribute != null)
                {
                    compare_str = attribute.Field;
                }

                var value = param.Get(property.PropertyType, compare_str);

                if (property.PropertyType.IsClass && param.IsObject(compare_str))
                {
                    Convert(value, property.PropertyType, param.Get<JObject>(compare_str));
                }

                property.SetValue(data, value);
            }

            return data;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldAttribute : System.Attribute
    {
        public string Field;

        public FieldAttribute(string field)
        {
            Field = field;
        }
    }
}
