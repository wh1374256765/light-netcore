using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using App.Server;

namespace App.Modules.User
{
    public class Route
    {
        Controller Controller = new Controller();

        public void Routes()
        {
            Router.Post("/user/login", Controller.Login);
            Router.Post("/user/register", Controller.Register);
        }
    }
}
