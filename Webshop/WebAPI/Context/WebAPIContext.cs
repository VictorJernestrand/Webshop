//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
//using Webshop.Models;

namespace WebAPI.Context
{
    public class WebAPIContext : DbContext
    {
        public WebAPIContext()
        {
            // ...
        }

        public WebAPIContext(DbContextOptions<WebAPIContext> options) : base(options)
        {
            // ...
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

    }
}
