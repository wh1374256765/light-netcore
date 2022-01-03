using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Common;
using App.Server;

namespace App.Modules.User {
    public class Controller : BaseController
    {
        public async Task Login(HttpContext ctx)
        {
            try
            {                
                var result = Service.Login(ctx.Request.GetBody());
                await ctx.Ok(result);
            }
            catch (Exception ex)
            {
                await ctx.Error(ex.Message);
            }

        }

        public async Task Register(HttpContext ctx)
        {
            try
            {                
                var result = RegisterService.Register(ctx.Request.GetBody());
                await ctx.Ok(result);
            }
            catch (Exception ex)
            {
                await ctx.Error(ex.Message);
            }

        }
    }
}
