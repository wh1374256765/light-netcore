using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using App.Common.excel.OpenXml;
using App.Common.excel.Utils;
using App.Common.excel.Zip;
using App.Server;
using App.Server.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Common
{
    public static class Excel
    {
        public static IWorkbook Load(IFormFile file)
        {
            IWorkbook workbook = null;
            using (var stream = file.OpenReadStream())
            {
                stream.Position = 0;
                workbook = WorkbookFactory.Create(stream);
            }

            return workbook;
        }

        public static IWorkbook LoadStream(Stream stream)
        {
            stream.Position = 0;
            var result = WorkbookFactory.Create(stream);

            return result;
        }

        public static string GetSheetName<T>()
        {
            var attribte = (SheetNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(SheetNameAttribute));

            if (attribte == null)
            {
                throw new Exception("SheetNameAttribute not found");
            }

            return attribte.SheetName;
        }

        public static bool ValidateHeader<T>(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(Excel.GetSheetName<T>());

            if (sheet.GetRow(0) == null)
            {
                throw new Exception("sheet header not found");
            }

            var property_list = typeof(T).GetProperties().ToList();

            var header_field_list = property_list.ConvertAll(property =>
            {
                var attribute = (SheetHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(SheetHeaderAttribute));
                if (attribute == null)
                {
                    return null;
                }
                return attribute.HeaderName;
            });

            var header_row = sheet.GetRow(0);

            for (var i = 0; i < header_row.Count(); i++)
            {
                ICell cell = header_row.GetCell(i);

                if (cell == null)
                {
                    return false;
                }

                var value = cell.StringCellValue;

                var header_field = header_field_list.Find(field => field != null && field == value);

                if (header_field == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static IWorkbook CreateSheet(IWorkbook workbook, string name, List<string> columns, List<object> data)
        {
            ISheet sheet = workbook.CreateSheet(name);

            Dictionary<string, int> header_dic = new Dictionary<string, int>();

            var header_row = sheet.CreateRow(0);

            for (var i = 0; i < columns.Count; i++)
            {
                var value = columns[i];

                header_dic[value] = i;

                header_row.CreateCell(i).SetCellValue(value);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var item_dic = item as IDictionary<string, object>;

                var row = sheet.CreateRow(i + 1);

                columns.ForEach(column =>
                {
                    var index = header_dic[column];

                    if (item_dic.ContainsKey(column) && item_dic[column] != null)
                    {
                        var cell = row.CreateCell(index);

                        cell.SetCellValue(item_dic[column].ToString());
                    }
                });
            }

            return workbook;
        }

        public static IWorkbook CreateSheet(IWorkbook workbook, string name, List<string> columns, List<IDictionary<string, object>> data)
        {
            ISheet sheet = workbook.CreateSheet(name);

            Dictionary<string, int> header_dic = new Dictionary<string, int>();

            var header_row = sheet.CreateRow(0);

            for (var i = 0; i < columns.Count; i++)
            {
                var value = columns[i];

                header_dic[value] = i;

                header_row.CreateCell(i).SetCellValue(value);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var item_dic = data[i];

                var row = sheet.CreateRow(i + 1);

                columns.ForEach(column =>
                {
                    var index = header_dic[column];

                    if (item_dic.ContainsKey(column) && item_dic[column] != null)
                    {
                        var cell = row.CreateCell(index);

                        cell.SetCellValue(item_dic[column].ToString());
                    }
                });
            }

            return workbook;
        }

        public static IWorkbook CreateSheet<T>(IWorkbook workbook, List<T> data)
        {
            ISheet sheet = workbook.CreateSheet(Excel.GetSheetName<T>());
            var property_list = typeof(T).GetProperties().ToList();

            Dictionary<string, int> header_dic = new Dictionary<string, int>();

            var header_row = sheet.CreateRow(0);

            for (var i = 0; i < property_list.Count; i++)
            {
                var property = property_list[i];

                var attribute = (SheetHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(SheetHeaderAttribute));
                if (attribute == null)
                {
                    continue;
                }
                header_dic[attribute.HeaderName] = i;

                header_row.CreateCell(i).SetCellValue(attribute.HeaderName);
            }

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var row = sheet.CreateRow(i + 1);
                for (var j = 0; j < property_list.Count; j++)
                {
                    var property = property_list[j];

                    var attribute = (SheetHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(SheetHeaderAttribute));
                    if (attribute == null)
                    {
                        continue;
                    }

                    var index = header_dic[attribute.HeaderName];
                    var cell = row.CreateCell(index);

                    var value = property.GetValue(item);

                    if (value == null)
                    {
                        continue;
                    }

                    if (property.PropertyType == typeof(decimal?) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(int?) || property.PropertyType == typeof(int))
                    {
                        cell.SetCellValue(Convert.ToDouble(value));
                    }

                    if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime))
                    {
                        var style = workbook.CreateCellStyle();
                        var format = workbook.CreateDataFormat();

                        style.DataFormat = format.GetFormat("yyyy-mm-dd");
                        cell.CellStyle = style;
                        cell.SetCellValue(Convert.ToDateTime(value));
                    }

                    if (property.PropertyType == typeof(string))
                    {
                        cell.SetCellValue(value.ToString());
                    }
                }
            }

            return workbook;
        }

        public static IWorkbook CreateWorkbook()
        {
            return new XSSFWorkbook();
        }

        public static List<Dictionary<string, object>> From(IWorkbook workbook, string sheetName)
        {
            var data = new List<Dictionary<string, object>>();

            ISheet sheet = workbook.GetSheet(sheetName);

            if (sheet.GetRow(0) == null)
            {
                throw new Exception("sheet header not found");
            }

            var header_row = sheet.GetRow(0);
            Dictionary<int, string> header_dic = new Dictionary<int, string>();

            for (var i = 0; i < header_row.Count(); i++)
            {
                ICell cell = header_row.GetCell(i);

                if (cell == null)
                {
                    continue;
                }

                var value = cell.StringCellValue;
                header_dic[i] = value;
            }

            foreach (IRow row in sheet)
            {
                if (row == sheet.GetRow(0))
                {
                    continue;
                }

                var item = new Dictionary<string, object>();
                for (var i = 0; i < header_dic.Keys.Count; i++)
                {
                    var cell = row.GetCell(i);
                    var header_name = header_dic[i];

                    if (cell == null)
                    {
                        item.Add(header_name, null);
                    }
                    else
                    {
                        if (cell.CellType == CellType.Numeric && cell.CellStyle.DataFormat != 0)
                        {
                            item.Add(header_name, cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            item.Add(header_name, cell.ToString());
                        }
                    }
                }

                data.Add(item);
            }

            return data;
        }

        public static List<T> From<T>(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(Excel.GetSheetName<T>());

            if (sheet.GetRow(0) == null)
            {
                throw new Exception("sheet header not found");
            }

            var header_row = sheet.GetRow(0);
            Dictionary<string, int> header_dic = new Dictionary<string, int>();

            for (var i = 0; i < header_row.Count(); i++)
            {
                ICell cell = header_row.GetCell(i);

                if (cell == null)
                {
                    continue;
                }

                var value = cell.StringCellValue;
                header_dic[value] = i;
            }

            var type = typeof(T);
            var property_arr = type.GetProperties();
            List<PropertyInfo> property_list = property_arr.Length <= 0 ? new List<PropertyInfo>() : property_arr.ToList();

            List<T> obj_list = new List<T>();

            foreach (IRow row in sheet)
            {
                if (row == sheet.GetRow(0))
                {
                    continue;
                }

                T obj = (T)Activator.CreateInstance(type);
                property_list.ForEach(property =>
                {
                    var attribute = (SheetHeaderAttribute)Attribute.GetCustomAttribute(property, typeof(SheetHeaderAttribute));
                    if (attribute == null)
                    {
                        return;
                    }

                    var header_name = attribute.HeaderName;
                    var value = row.GetCell(header_dic[header_name]);

                    if (property.PropertyType == typeof(ICell))
                    {
                        property.SetValue(obj, value);
                    }

                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(obj, value.ToString());
                    }

                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        property.SetValue(obj, Convert.ToInt32(value.NumericCellValue));
                    }

                    if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                    {
                        property.SetValue(obj, Convert.ToDouble(value.NumericCellValue));
                    }

                    if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                    {
                        property.SetValue(obj, Convert.ToDecimal(value.NumericCellValue));
                    }

                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        property.SetValue(obj, value.DateCellValue);
                    }
                });

                obj_list.Add(obj);
            }

            return obj_list;
        }
    }
}
