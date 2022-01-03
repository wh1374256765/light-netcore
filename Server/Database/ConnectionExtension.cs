using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Server
{
    public static class ConnectionExtension
    {
        public static void Open(this IDbConnection conn, int timeout)
        {
            Stopwatch sw = new Stopwatch();
            bool connectSuccess = false;

            Thread t = new Thread(delegate ()
            {
                try
                {
                    sw.Start();
                    conn.Open();
                    connectSuccess = true;
                }
                catch
                {

                }
            });

            t.IsBackground = true;
            t.Start();

            while (timeout > sw.ElapsedMilliseconds)
            {
                if (t.Join(1))
                {
                    break;
                }
            }

            if (!connectSuccess)
            {
                throw new Exception("Timed out while trying to connect.");
            }
        }
    }
}
