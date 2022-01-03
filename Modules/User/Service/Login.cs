

using Dapper;
using App;
using Microsoft.AspNetCore.Mvc;
using dotnet = System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using App.Common;
using Newtonsoft.Json.Linq;
using System;
using App.Crypto;
using Entity = App.Server.Database.Entity;
using App.Server;
using Microsoft.Extensions.Configuration;

namespace App.Modules.User
{
    public static partial class Service
    {
        public static object Login(dynamic data)
        {
            LoginContext ctx = new LoginContext(data);

            ctx.Valid();

            var user = ctx.GetUser();
            ctx.ValidPassword(user.Password);

            var token = ctx.GenToken(user);
            ctx.UpdateUserToken(user, token);

            return new
            {
                Id = user.Id,
                Username = user.UserName,
                Token = token
            };
        }

        public class LoginContext : BaseRequest
        {
            [Field("username")]
            public string UserName { get; set; }

            public string Password { get; set; }

            public bool Valid()
            {
                return true;
            }

            public LoginContext(JObject data)
            {
                this.From(data);
            }

            public void ValidPassword(string db_password)
            {
                var salt = "smartpath";

                var encrpt_password = MD5.Encode(this.Password, salt);

                if (encrpt_password != db_password)
                {
                    throw new Exception("密码不正确");
                }
            }

            public Entity.User GetUser()
            {
                var table_name = this.Db.GetTableName<Entity.User>();

                var user = this.Db.QueryOne<Entity.User>($"select Id, UserName, Password from {table_name} where UserName=@UserName", new
                {
                    UserName = this.UserName
                });

                if (user == null)
                {
                    throw new Exception("用户名不存在，请先注册");
                }

                return user;
            }

            public string GenToken(Entity.User user)
            {
                var priv_token = Config.Get().GetSection("JwtToken").Get<string>();

                var token = Jwt.Encode(priv_token, new
                {
                    id = user.Id,
                    userName = user.UserName,
                    password = user.Password,
                    createTime = DateTime.Now
                });

                return token;
            }

            public void UpdateUserToken(Entity.User user, string token)
            {
                var table_name = this.Db.GetTableName<Entity.User>();

                this.Db.Execute($"update {table_name} set Token=@Token where Id=@Id", new
                {
                    Token = token,
                    Id = user.Id
                });
            }
        }
    }
}

