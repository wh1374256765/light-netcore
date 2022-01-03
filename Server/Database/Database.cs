using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace App.Server.Database
{
    public class Database
    {
        public DatabaseConfig Option { get; set; }

        public class ParamSql
        {
            public string Sql { get; set; }
            public object Params { get; set; }
        }

        public string GetTableName<T>()
        {
            var attribte = (TableNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableNameAttribute));
            return attribte.TableName;
        }

        public IDbConnection GetConnection()
        {
            IDbConnection connection = typeFuncDic[this.Option.Type](this.Option.ConnectionString);
            return connection;
        }

        public IDbConnection GetConnection(string db_type, string connection_str)
        {
            IDbConnection connection = typeFuncDic[db_type](connection_str);
            return connection;
        }

        public List<string> GetFields<T>()
        {
            Type type = typeof(T);

            Dictionary<string, object> property_dic = new Dictionary<string, object>();
            var fields = type.GetProperties().ToList().ConvertAll(property => property.Name);

            return fields;
        }

        public object Insert<T>(T data)
        {
            var table_name = this.GetTableName<T>();
            var fields = this.GetFields<T>();

            var field_str = String.Join(',', fields.ConvertAll((field) => $"`{field}`"));
            var field_param_str = String.Join(',', fields.ConvertAll((field) => $"@{field}"));

            var sql = $"insert into {table_name} ({field_str}) values({field_param_str});select LAST_INSERT_ID();";

            using (var connection = this.GetConnection())
            {
                connection.Open();
                return connection.ExecuteScalar(sql, data);
            }
        }

        public DataTable Query(string sql, object param = null)
        {
            var data_table = new DataTable();

            using (var connection = this.GetConnection())
            {
                connection.Open();

                using (var reader = connection.ExecuteReader(sql, param))
                {
                    data_table.Load(reader);
                }
            }

            return data_table;
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            if (typeof(T) == typeof(IDictionary<string, object>))
            {
                var result_dic = new List<Dictionary<string, object>>();

                using (var connection = this.GetConnection())
                {
                    connection.Open();

                    using (var reader = connection.ExecuteReader(sql, param))
                    {
                        while (reader.Read())
                        {                            
                            var dict = new Dictionary<string, object>();

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                dict[reader.GetName(i)] = reader.GetValue(i);
                            }

                            result_dic.Add(dict);
                        }
                    }
                }

                return result_dic as IEnumerable<T>;
            }

            IEnumerable<T> result = default(IEnumerable<T>);

            using (var connection = this.GetConnection())
            {
                connection.Open();
                result = connection.Query<T>(sql, param);
            }

            return result;
        }

        public IDataReader GetReader(string sql, object param = null)
        {
            var connection = this.GetConnection();

            connection.Open();
            var reader = connection.ExecuteReader(sql, param);
            connection.Close();

            return reader;
        }

        public T QueryOne<T>(string sql, object param = null)
        {
            T result = default(T);

            using (var connection = this.GetConnection())
            {
                connection.Open();
                result = connection.QueryFirstOrDefault<T>(sql, param);
            }

            return result;
        }

        public void Execute(string sql, object param = null)
        {
            using (var connection = this.GetConnection())
            {
                connection.Open();
                connection.Execute(sql, param);
            }
        }

        public void Execute(string sql, List<object> data)
        {
            if (data == null)
            {
                data = new List<object>();
            }

            using (var connection = this.GetConnection())
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction();

                connection.Execute(sql, data.ToArray(), transaction);

                transaction.Commit();
            }
        }

        public void Execute(List<string> sql_list)
        {
            if (sql_list == null)
            {
                sql_list = new List<string>();
            }

            using (var connection = this.GetConnection())
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction();

                sql_list.ForEach(sql =>
                {
                    connection.Execute(sql, null, transaction);
                });

                transaction.Commit();
            }
        }

        public void Execute(List<ParamSql> param_sql_list)
        {
            if (param_sql_list == null)
            {
                return;
            }

            using (var connection = this.GetConnection())
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction();

                param_sql_list.ForEach(param_sql =>
                {
                    connection.Execute(param_sql.Sql, param_sql.Params, transaction);
                });

                transaction.Commit();
            }
        }

        public void Insert<T>(List<T> data)
        {
            var table_name = this.GetTableName<T>();
            var fields = this.GetFields<T>();

            var field_str = String.Join(',', fields.ConvertAll((field) => $"`{field}`"));
            var field_param_str = String.Join(',', fields.ConvertAll((field) => $"@{field}"));

            var sql = $"insert into {table_name} ({field_str}) values({field_param_str})";

            using (var connection = this.GetConnection())
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction();

                connection.Execute(sql, data.ToArray(), transaction);

                transaction.Commit();
            }
        }

        public object GetLastId<T>()
        {
            object id = null;
            var table_name = this.GetTableName<T>();
            var sql = $"select Id from {table_name} order by Id DESC limit 1";

            using (var connection = this.GetConnection())
            {
                connection.Open();

                id = connection.QueryFirstOrDefault<object>(sql);
            }

            return id;
        }

        static Dictionary<string, Database> databases = new Dictionary<string, Database>();
        static Database defaultDatabase;

        static Dictionary<string, Func<string, IDbConnection>> typeFuncDic = new Dictionary<string, Func<string, IDbConnection>>()
        {
            { "MySql", (connectionString) =>
                {
                    return new MySqlConnection(connectionString);
                }
            },
            { "SqlServer", (connectionString) =>
                {
                    return new SqlConnection(connectionString);
                }
            }
        };

        static public void Init(IConfiguration config)
        {
            var configs = config.GetSection("Databases").Get<List<DatabaseConfig>>();

            configs.ForEach(config =>
            {
                Database db = new Database();
                db.Option = config;

                using (var connection = db.GetConnection())
                {
                    connection.Open();
                    Logger.Info($"database [{config.Name}] connected.");
                }

                databases[config.Name] = db;

                if (config.IsDefault)
                {
                    defaultDatabase = db;
                }
            });
        }

        static public Database Create(DatabaseConfig config)
        {
            Database db = new Database();
            db.Option = config;

            using (var connection = db.GetConnection())
            {
                connection.Open();
                Logger.Info($"database [{config.Name}] connected.");
            }

            return db;
        }

        static public Database Get(string name)
        {
            return databases[name];
        }

        static public Database GetDefault()
        {
            return defaultDatabase;
        }
    }
}
