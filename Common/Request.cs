using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Common
{
    public static class Request
    {
        public static Stream Download(string url)
        {
            WebRequest client = WebRequest.Create(url);
            client.Method = "GET";
            var res = client.GetResponse();
            var memory_stream = new MemoryStream();
            var stream = res.GetResponseStream();
            stream.CopyTo(memory_stream);
            stream.Dispose();
            return memory_stream;
        }

        public static ExpandoObject Get(string url)
        {
            WebRequest client = WebRequest.Create(url);
            client.Method = "GET";
            var res = client.GetResponse();
            var stream = res.GetResponseStream();
            var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<ExpandoObject>(json);

            reader.Dispose();
            stream.Dispose();

            return result;
        }
    }
}
