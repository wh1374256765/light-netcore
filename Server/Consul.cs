using Consul;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Server
{
    public static class Consul
    {
        static ConsulClient ConsulClient;

        static ConsulConfig Option;

        static AgentServiceRegistration AgentServiceRegistration;

        static public void Init(IConfiguration config)
        {
            var consul_config = config.GetSection("Consul").Get<ConsulConfig>();

            Option = consul_config;

            if (consul_config.Url == null || consul_config.ServiceUrl == null)
            {
                Logger.Error("consul config must have url and Service Url");
                return;
            }

            var host = "";
            var port = "";

            if (consul_config.ServiceUrl.LastIndexOf(":") < 0) {
                host = consul_config.ServiceUrl;
                port = "80";
            } else {
                host = consul_config.ServiceUrl.Substring(0, consul_config.ServiceUrl.LastIndexOf(":"));
                port = consul_config.ServiceUrl.Substring(consul_config.ServiceUrl.LastIndexOf(":") + 1);
            }

            var consul_client = new ConsulClient(client =>
            {
                client.Address = new Uri(consul_config.Url);
            });

            ConsulClient = consul_client;

            AgentServiceRegistration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),
                Name = consul_config.ServiceName,
                Address = host,
                Port = int.Parse(port),
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(20),
                    HTTP = $"http://{consul_config.ServiceUrl}{consul_config.HealthCheckUrl}",
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            Consul.Open();
        }

        static public void Open()
        {
            ConsulClient.Agent.ServiceRegister(AgentServiceRegistration).Wait();
        }

        static public void Close()
        {
            ConsulClient.Agent.ServiceDeregister(AgentServiceRegistration.ID).Wait();
        }

        static public JObject GetService(string serviceName, string url)
        {
            var test = ConsulClient.Catalog.Service(serviceName);
            var services = ConsulClient.Catalog.Service(serviceName).Result.Response;

            if (services == null || !services.Any())
            {
                Logger.Error("service not found");
                return null;
            }

            var service = services.ElementAt(0);

            using (HttpClient client = new HttpClient())
            {
                var res = client.GetAsync($"http://{service.ServiceAddress}:{service.ServicePort}{url}").Result;

                string result = res.Content.ReadAsStringAsync().Result;

                var data = JsonConvert.DeserializeObject<JObject>(result);

                return data;
            }
        }
    }

    public class ConsulConfig
    {
        public string Url { get; set; }

        public string ServiceName { get; set; }

        public string ServiceUrl { get; set; }

        public string HealthCheckUrl { get; set; }
    }
}
