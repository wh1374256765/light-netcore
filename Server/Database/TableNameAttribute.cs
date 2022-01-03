using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace App.Server.Database
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TableNameAttribute : System.Attribute
    {
        public string TableName;

        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
