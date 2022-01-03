using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Common
{
    public static class JobjectExtension
    {
        public static T Get<T>(this JObject data, string key)
        {
            return (data == null || data[key] == null || data[key].Type == JTokenType.Null) ? default(T) : data[key].ToObject<T>();
        }

        public static object Get(this JObject data, Type type, string key)
        {
            var default_value = type.IsValueType ? Activator.CreateInstance(type) : null;
            return (data == null || data[key] == null || data[key].Type == JTokenType.Null) ? default_value : data[key].ToObject(type);
        }

        public static Boolean IsObject(this JObject data, string key)
        {
            return data != null && data[key] != null && data[key].Type == JTokenType.Object;
        }
    }
}