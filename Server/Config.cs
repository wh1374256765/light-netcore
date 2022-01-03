using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Server
{
    public static class Config
    {
        private static IConfiguration Configuration;

        public static void Init(IConfiguration configuration) {
            Configuration = configuration;
        }

        public static IConfiguration Get() {
            if (Configuration == null) {
                Logger.Error("Config haven't initialized");
            }

            return Configuration;
        }
    }
}
