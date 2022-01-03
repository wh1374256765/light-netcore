using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace App.Server
{
    public class Router
    {
        public RouteBuilder Builder;
        public IApplicationBuilder App;

        public static string Prefix { get; set; }

        public Router(IApplicationBuilder app)
        {
            App = app;
            Builder = new RouteBuilder(app);
            Prefix = "";
        }

        static List<RouteData> Routes = new List<RouteData>();

        public static void Post(string url, RequestDelegate function)
        {
            Routes.Add(new RouteData()
            {
                Method = "post",
                Url = Prefix + url,
                Function = function
            });
        }

        public static void Get(string url, RequestDelegate function)
        {
            Routes.Add(new RouteData()
            {
                Method = "get",
                Url = Prefix + url,
                Function = function
            });
        }

        public void Run()
        {
            Routes.ForEach(route =>
            {
                if (route.Method == "get")
                {
                    Builder.MapGet(route.Url, route.Function);
                }
                if (route.Method == "post")
                {
                    Builder.MapPost(route.Url, route.Function);
                }
            });

            App.UseRouter(Builder.Build());
        }
    }

    public class RouteData
    {
        public string Method { get; set; }

        public string Url { get; set; }

        public RequestDelegate Function { get; set; }
    }
}
