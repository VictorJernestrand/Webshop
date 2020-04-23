using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddTransient<WebAPIToken>();
            services.AddTransient<WebAPIHandler>();

            // Needed for IHttpClientFactory to work and accessing our WebAPI
            services.AddHttpClient();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddMvcCore().AddAuthorization(); // Note - this is on the IMvcBuilder, not the service collection

            services.AddSession(); // Enable session cookies

            services.AddSingleton(_ => Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Catch unhandled exceptions
                app.UseExceptionHandler("/Error");

                // Catch 404 etc errors
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // Enable session cookies. Must be added BEFORE .UseEndpoints!!!

            app.UseCookiePolicy();

            app.UseAuthentication(); // Identifies who is who. For Identity features. Must be added BEFORE Authorization!!!

            app.UseAuthorization(); // Give different users access to different areas in the application.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Activate SSL
            app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());
        }
    }
}
