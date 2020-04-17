using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Context;
using WebAPI.Models;
using WebAPI.Models.Data;
using WebAPI.Services;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string _corsePolicyString = "WebshopWebAPI";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get connectionstring from appsettings.json
            services.AddDbContext<WebAPIContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlDatabase"));
            });

            // Set email to be unique for each user
            services.AddIdentity<User, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            }).AddRoles<AppRole>()
              .AddEntityFrameworkStores<WebAPIContext>();

            // Add to independancy injection
            services.AddTransient<CustomerOrderService>();
            services.AddTransient<MailService>();

            services.AddCors(options =>
            {
                //options.AddPolicy(_corsePolicyString,
                //    builder =>
                //    {
                //        builder.WithOrigins("https://localhost:44364")
                //               .AllowAnyHeader()
                //               .AllowAnyMethod();
                //    });
                options.AddPolicy(_corsePolicyString,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebAPIContext context, UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

          

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_corsePolicyString);

            app.UseAuthentication();    // Identifies who is who. For Identity features. Must be added BEFORE Authorization!!!

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Activate SSL
            app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());

            // Add a default Admin account and Roles!
            AdminAccountAndRoles.Initialize(context, userManager, roleManager).Wait();
        }
    }
}
