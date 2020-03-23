using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set the SqlServer to use the connection string form appsettings.json
            services.AddDbContext<WebshopContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlDatabase"));
            });

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddTransient<TokenRequest>();
            services.AddSingleton<WebAPIHandler>();


            // Set email to be unique for each user
            services.AddIdentity<User, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddRoles<AppRole>()
              .AddEntityFrameworkStores<WebshopContext>();

            // Configure the application cookie and set expiration date
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            // Needed for IHttpClientFactory to work and accessing our WebAPI
            services.AddHttpClient();

            services.AddMvcCore().AddAuthorization(); // Note - this is on the IMvcBuilder, not the service collection

            services.AddSession(); // Enable session cookies
            services.AddControllersWithViews();
            services.AddSingleton(_ => Configuration);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebshopContext context, UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();           // Enable session cookies. Must be added BEFORE .UseEndpoints!!!
            app.UseCookiePolicy();

            app.UseAuthentication();    // Identifies who is who. For Identity features. Must be added BEFORE Authorization!!!
            app.UseAuthorization();     // Give different users access to different areas in the application.


            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "test",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });*/

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Activate SSL
            app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());

            // Add a default Admin account and Roles!
            AdminAccountAndRoles.Initialize(context, userManager, roleManager).Wait();
        }
    }
}
