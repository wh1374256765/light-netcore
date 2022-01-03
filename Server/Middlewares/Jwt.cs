using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Server.Middlewares
{
    public class Jwt
    {
        private readonly RequestDelegate _next;
        private List<string> _white_list;

        public Jwt(RequestDelegate next, List<string> white_list)
        {
            _white_list = white_list;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {            
            if (_white_list.Contains(context.Request.Path.Value))
            {
                await _next(context);
                return;
            }

            var authorization = context.Request.Headers["authorization"].ToString();

            if (authorization == "")
            {
                await context.Error(403, "no authorization");
                return;
            }

            authorization = authorization.Replace("Bearer ", "");
            var priv_token = Config.Get().GetSection("JwtToken").Get<string>();
            var result = Server.Jwt.Decode(authorization, priv_token);

            if (result == null) {
                await context.Error(403, "no authorization");
                return;
            }

            await _next(context);
        }
    }
}
