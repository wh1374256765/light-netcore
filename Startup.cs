using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Modules.System.Schedule;
using App.Server;
using App.Server.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Logger.init();

            // init config

            Config.Init(configuration);
            // Database.Init(configuration);
            // Ftp.Init(configuration);

            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseRequest();

            app.UseAuthorization();

            app.UseJwt(new List<string>()
            {
                "/admin/api/user/login",
                "/admin/api/user/register"
            });

            app.UseEndpoints(endpoints =>
            {
                Router router = new Router(app);

                Router.Prefix = "/admin/api";
                Modules.Main.Route();

                router.Run();

                Modules.Main.ClearMemory();
            });

            lifetime.ApplicationStopping.Register(() =>
            {
                // when application closed, you want close some service
            });
        }
    }
}
