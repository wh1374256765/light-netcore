using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using App.Server;
using App.Server.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App.Crypto
{
    public static class MD5
    {
        public static string Encode(string str, string salt)
        {
            str = $"{str}{salt}";

            var md5 = new MD5CryptoServiceProvider();
            var str_bytes = Encoding.Default.GetBytes(str);
            var encrypt_str_bytes = md5.ComputeHash(str_bytes);

            var encrypt_str = "";
            foreach (byte item in encrypt_str_bytes)
            {
                encrypt_str += item.ToString("x2");
            }

            return encrypt_str;
        }
    }
}
