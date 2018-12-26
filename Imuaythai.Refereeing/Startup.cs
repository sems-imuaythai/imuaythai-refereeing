using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imuaythai.Refereeing.Hubs;
using Imuaythai.Refereeing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imuaythai.Refereeing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR();

            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
             {
                 builder.AllowAnyMethod().AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowCredentials();
             }));

            services.AddTransient<IFightService, FightService>();
            services.AddTransient<IFightStorage, RedisFightStorage>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSignalR(config =>
               {
                   config.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/a"));
                   config.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/b"));
                   config.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/c"));
               });

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
