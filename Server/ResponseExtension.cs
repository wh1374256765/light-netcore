using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Server
{
    public static class ResponseExtension
    {
        public static async Task Json<T>(
           this HttpResponse response, T data)
        {
            response.StatusCode = 200;
            response.ContentType = "application/json";

            await response.WriteAsJsonAsync(data);
        }
    }
}
