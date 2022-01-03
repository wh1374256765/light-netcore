using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.Server
{
    public static class HttpContextExtension
    {
        public static async Task Ok<T>(
           this HttpContext context, T data)
        {
            MessageModel<T> result = new MessageModel<T>();
            result.status = 200;
            result.msg = "success";
            result.success = true;
            result.response = data;

            var data_str = JsonConvert.SerializeObject(result);
            Logger.Info($"response data: {data_str}");

            await context.Response.Json(result);
        }

        public static async Task Error(
           this HttpContext context, string msg)
        {
            MessageModel<string> result = new MessageModel<string>();
            result.status = 200;
            result.msg = msg;
            result.success = false;
            result.response = null;

            var data_str = JsonConvert.SerializeObject(result);
            Logger.Error($"response data: {data_str}");

            await context.Response.Json(result);
        }

        public static async Task Error(this HttpContext context, int code, string msg)
        {
            MessageModel<string> result = new MessageModel<string>();
            result.status = code;
            result.msg = msg;
            result.success = false;
            result.response = null;

            var data_str = JsonConvert.SerializeObject(result);
            Logger.Error($"response data: {data_str}");

            await context.Response.Json(result);
        }

        public static async Task Redirect(this HttpContext context, string url)
        {
            var request = context.Request;

            if (request.Method == "GET")
            {
                var query_list = new List<string>();
                foreach (var item in request.Query)
                {
                    var value = item.Value[0];
                    query_list.Add($"{item.Key}={value}");
                }

                if (query_list.Count() > 0)
                {
                    var query_str = string.Join('&', query_list);
                    url += $"?{query_str}";
                }
            }

            WebRequest client = WebRequest.Create(url);

            foreach (var item in request.Headers)
            {
                if (!WebHeaderCollection.IsRestricted(item.Key))
                {
                    client.Headers.Add(item.Key, item.Value);
                }
            }

            if (request.Method == "POST")
            {
                client.Method = request.Method;
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var stream = client.GetRequestStream())
                {
                    using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                    {

                        reader.BaseStream.CopyTo(stream);
                        request.Body.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            var res = client.GetResponse();

            using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            {
                string res_str = reader.ReadToEnd();
                var data = System.Text.Json.JsonSerializer.Deserialize<object>(res_str);
                await context.Response.Json(data);
            }
        }
    }
}
