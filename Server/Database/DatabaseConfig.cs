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
    public class DatabaseConfig
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string ConnectionString { get; set; }

        public Boolean IsDefault { get; set; }
    }
}
