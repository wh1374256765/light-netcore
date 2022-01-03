using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Common;
using App.Server;

namespace App.Modules.HealthCheck
{
    public class Controller : BaseController
    {
        public async Task Get(HttpContext ctx)
        {
            await ctx.Response.Json("ok");
        }        
    }
}
