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
    public static class DateTimeExtension
    {
        public static long ToTimestamp(this DateTime time)
        {
            System.DateTime startTime = DateTime.MinValue;
            long timeStamp = (long)(time - startTime).TotalSeconds;
            return timeStamp;
        }
    }
}
