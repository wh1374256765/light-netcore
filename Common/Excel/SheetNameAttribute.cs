using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace App.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SheetNameAttribute : System.Attribute
    {
        public string SheetName;

        public SheetNameAttribute(string sheetName)
        {
            SheetName = sheetName;
        }
    }
}
