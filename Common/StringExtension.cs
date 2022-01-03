using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Common
{
    public static class StringExtension
    {
        public static string ToUnderScore(this String str)
        {
            var result = "";
            result += str.Substring(0, 1).ToLower();

            var regex_str = "([A-Z])";

            result += Regex.Replace(str.Substring(1), regex_str, "_$0").ToLower();

            return result;
        }

        public static string ToCamelcase(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
