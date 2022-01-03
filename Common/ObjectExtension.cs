using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Common
{
    public static class ObjectExtension
    {
        public static int ToInt(object obj)
        {
            int value = 0;

            try
            {
                value = Convert.ToInt32(obj);
            }
            catch
            {
                value = 0;
            }

            return value;
        }
    }
}
