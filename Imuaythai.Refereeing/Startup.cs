using Imuaythai.Refereeing.Hubs;
using Imuaythai.Refereeing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 

using Microsoft.AspNetCore.Http; 

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();

            services.AddTransient<IRoundTimer, RoundTimer>();
            services.AddTransient<IBreaksTimer, BreaksTimer>();
            services.AddTransient<IWinnerCalculator, WinnerCalculator>();
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalR(routes =>
            {
                routes.MapHub<RingHub>("/chatHub");
                routes.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/a"));
                routes.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/b"));
                routes.MapHub<RingHub>(new Microsoft.AspNetCore.Http.PathString("/ring/c"));
            }); 
            app.UseMvc();
        }
    }
}
