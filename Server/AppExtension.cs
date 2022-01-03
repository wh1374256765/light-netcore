using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Server
{
    public static class AppExtension
    {
        public static IApplicationBuilder UseRequest(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middlewares.Request>();
        }

        public static IApplicationBuilder UseJwt(
           this IApplicationBuilder builder, List<string> white_list)
        {
            return builder.UseMiddleware<Middlewares.Jwt>(white_list);
        }
    }
}
