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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SheetHeaderAttribute : System.Attribute
    {
        public string HeaderName;

        public SheetHeaderAttribute(string headerName)
        {
            HeaderName = headerName;
        }
    }
}
