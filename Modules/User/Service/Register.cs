using App.Common;
using Newtonsoft.Json.Linq;
using System;
using App.Crypto;
using Entity = App.Server.Database.Entity;
using App.Server;
using Microsoft.Extensions.Configuration;

namespace App.Modules.User
{
    public static partial class RegisterService
    {
        public static object Register(dynamic data)
        {
            RegisterContext ctx = new RegisterContext(data);

            ctx.Valid();

            ctx.CryptPassword();

            var user = ctx.SaveUser();

            return new
            {
                Id = user.Id,
                Username = user.UserName,
                Token = user.Token
            };
        }

        public class RegisterContext : BaseRequest
        {
            [Field("username")]
            public string UserName { get; set; }

            public string Password { get; set; }

            public bool Valid()
            {
                this.Require("UserName", "Password");
                this.ValidPassword();
                this.ValidUserExist();
                return true;
            }

            public RegisterContext(JObject data)
            {
                this.From(data);
            }

            public void ValidPassword()
            {
                if (this.Password.Length < 6 || this.Password.Length > 36)
                {
                    throw new Exception("密码长度必须在6-36个字符之间");
                }
            }

            public void ValidUserExist()
            {
                var table_name = this.Db.GetTableName<Entity.User>();

                var user = this.Db.QueryOne<Entity.User>($"select Id, UserName, Password from {table_name} where UserName=@UserName", new
                {
                    UserName = this.UserName
                });

                if (user != null)
                {
                    throw new Exception("用户已存在");
                }
            }

            public void CryptPassword()
            {
                var salt = "smartpath";
                this.Password = MD5.Encode(this.Password, salt);
            }

            public Entity.User SaveUser()
            {
                var model = new Entity.User();
                model.UserName = this.UserName;
                model.Password = this.Password;
                model.CreateBy = null;
                model.CreateByName = null;
                model.CreateTime = DateTime.Now;
                model.UpdateBy = null;
                model.UpdateByName = null;
                model.UpdateTime = DateTime.Now;

                var id = this.Db.Insert(model);

                model.Id = Convert.ToInt32(id);

                var table_name = this.Db.GetTableName<Entity.User>();

                var priv_token = Config.Get().GetSection("JwtToken").Get<string>();
                var token = Jwt.Encode(priv_token, new
                {
                    id = id,
                    userName = this.UserName,
                    password = this.Password,
                    createTime = DateTime.Now
                });

                model.Token = token;

                this.Db.Execute($"update {table_name} set Token=@Token where Id=@Id", new
                {
                    Token = token,
                    Id = id
                });

                return model;
            }
        }
    }
}