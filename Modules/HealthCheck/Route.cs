using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using App.Server;

namespace App.Modules.HealthCheck
{
    public class Route
    {
        Controller Controller = new Controller();

        public void Routes()
        {
            Router.Get("/healthcheck", Controller.Get);
        }
    }
}
