using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using App.Server.Database.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Server.Middlewares
{
    public class Request
    {
        private readonly RequestDelegate _next;

        public Request(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            var token = request.Headers["x-token"].ToString().Replace("\"", "");

            if (token != "")
            {
                var db = Database.Database.GetDefault();
                var table_name = db.GetTableName<User>();
                var user = db.QueryOne<User>($"select Token from {table_name} where Token=@Token", new
                {
                    Token = token
                });

                if (user == null)
                {
                    await context.Error(403, "no authorization");
                    return;
                }

                request.Headers.Add("authorization", new StringValues($"Bearer {token}"));
            }

            if (request.Method == "GET")
            {
                var query = request.Query;

                Dictionary<string, object> query_dic = new Dictionary<string, object>();

                foreach (var item in query)
                {
                    query_dic.Add(item.Key, item.Value[0]);
                }

                var request_str = JsonConvert.SerializeObject(query_dic);

                Logger.Info($"request query: {request_str}");
            }

            if (request.Method == "POST")
            {
                if (request.Headers["Content-Type"].ToString().Contains("multipart/form-data"))
                {
                    var form_data = request.Form;
                    var form_data_dic = new Dictionary<string, object>();

                    if (form_data.Files.Count > 0)
                    {
                        foreach (var form_data_item in form_data.Files)
                        {
                            form_data_dic.Add(form_data_item.Name, form_data_item.FileName);
                        }
                    }

                    if (form_data.Keys.Count > 0)
                    {
                        foreach (var form_data_item in form_data)
                        {
                            form_data_dic.Add(form_data_item.Key, form_data_item.Value[0]);
                        }
                    }

                    var request_str = JsonConvert.SerializeObject(form_data_dic);
                    Logger.Info($"request form-data: {request_str}");
                }
                else
                {
                    request.EnableBuffering();

                    using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                    {

                        var request_str = await reader.ReadToEndAsync();

                        request.Body.Seek(0, SeekOrigin.Begin);

                        request_str = request_str.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "");

                        Logger.Info($"request body: {request_str}");
                    }
                }
            }

            await _next(context);
        }
    }
}
