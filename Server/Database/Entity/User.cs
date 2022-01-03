using Microsoft.AspNetCore.Builder;
using App.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Server.Database.Entity
{
    [TableName("user_info")]
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public int? CreateBy { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateByName { get; set; }

        public string UpdateByName { get; set; }
    }
}
