using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace App.Modules
{
    public class Main
    {
        public static void Route()
        {
            var routes = Assembly.GetAssembly(typeof(Main)).GetTypes().Where(type => type.Name == "Route").ToList();

            if (routes != null && routes.Any())
            {
                routes.ForEach(route =>
                {
                    route.GetMethod("Routes").Invoke(Activator.CreateInstance(route), null);
                });
            }
        }

        public static void ClearMemory() {
            Task.Run(() => {
                while(true) {
                    Thread.Sleep(5000);
                    GC.Collect();
                }
            });
        }
    }
}
