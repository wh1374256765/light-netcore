using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Server
{
    public static class RequestExtension
    {
        public static JObject GetBody(
           this HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                var request_str = reader.ReadToEnd();            

                request.Body.Seek(0, SeekOrigin.Begin);

                var data = JsonConvert.DeserializeObject<JObject>(request_str);

                var authorization = request.Headers["authorization"].ToString();

                if (authorization != "")
                {
                    try
                    {
                        authorization = authorization.Replace("Bearer ", "");
                        var priv_token = Config.Get().GetSection("JwtToken").Get<string>();
                        var result = Server.Jwt.Decode(authorization, priv_token);

                        var result_jtoken = JsonConvert.DeserializeObject<JToken>(result);

                        if (result != null)
                        {
                            data.Add("user", result_jtoken);
                        }
                    }
                    catch
                    {
                    }
                }

                return data;
            }
        }

        public static JObject GetQuery(
           this HttpRequest request)
        {
            Dictionary<string, object> query_dic = new Dictionary<string, object>();

            foreach (var item in request.Query)
            {
                query_dic.Add(item.Key, item.Value[0]);
            }

            var authorization = request.Headers["authorization"].ToString();

            if (authorization != "")
            {
                try
                {
                    authorization = authorization.Replace("Bearer ", "");
                    var priv_token = Config.Get().GetSection("JwtToken").Get<string>();
                    var result = Server.Jwt.Decode(authorization, priv_token);

                    if (result != null)
                    {
                        query_dic.Add("user", JsonConvert.DeserializeObject(result));
                    }
                }
                catch
                {
                }
            }

            var query_str = JsonConvert.SerializeObject(query_dic);

            var data = JsonConvert.DeserializeObject<JObject>(query_str);

            return data;
        }

        public static JObject GetFormData(this HttpRequest request)
        {
            Dictionary<string, object> query_dic = new Dictionary<string, object>();

            foreach (var item in request.Form)
            {
                query_dic.Add(item.Key, item.Value[0]);
            }

            var authorization = request.Headers["authorization"].ToString();

            if (authorization != "")
            {
                try
                {
                    authorization = authorization.Replace("Bearer ", "");
                    var priv_token = Config.Get().GetSection("JwtToken").Get<string>();
                    var result = Server.Jwt.Decode(authorization, priv_token);

                    if (result != null)
                    {
                        query_dic.Add("user", JsonConvert.DeserializeObject(result));
                    }
                }
                catch
                {
                }
            }
            var query_str = JsonConvert.SerializeObject(query_dic);
            var data = JsonConvert.DeserializeObject<JObject>(query_str);

            return data;
        }
    }
}
