using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Context
{
    public class WebshopContext : IdentityDbContext<User, AppRole, int>
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Status> Statuses { get; set; }

        public WebshopContext()
        {
            // ....
        }

        public WebshopContext(DbContextOptions<WebshopContext> options) : base(options)
        {
            // ...
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductOrder>(entity =>
            {
                entity.Property(x => x.Discount).HasDefaultValue(0);
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(x => x.FirstName).HasMaxLength(30);
                entity.Property(x => x.LastName).HasMaxLength(30);
                entity.Property(x => x.StreetAddress).HasMaxLength(50);
                entity.Property(x => x.ZipCode).HasMaxLength(5);
                entity.Property(x => x.City).HasMaxLength(50);
            });

            builder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Price).IsRequired();
                entity.Property(x => x.Quantity).IsRequired();
            });

            builder.Entity<Category>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(30);
            });

            builder.Entity<Brand>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(30);
            });

            builder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(20);
            });

            builder.Entity<Status>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(30);
            });


            builder.Entity<Brand>().HasData(
                new Brand() { Id = 1, Name = "Gibson" },
                new Brand() { Id = 2, Name = "Fender" },
                new Brand() { Id = 3, Name = "Yamaha" },
                new Brand() { Id = 4, Name = "Korg" },
                new Brand() { Id = 5, Name = "Millenium" }
            );

            builder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Drum set" },
                new Category() { Id = 2, Name = "Bas" },
                new Category() { Id = 3, Name = "Piano" },
                new Category() { Id = 4, Name = "Keyboard" },
                new Category() { Id = 5, Name = "Guitar" }
            );

            builder.Entity<Product>().HasData(
                new Product() { Id = 1, Name = "Stratocaster", Price = 4000, Quantity = 4, CategoryId = 5, Description = "Black and white", BrandId = 2,Photo= @"Guitar\guitar1" },
                new Product() { Id = 2, Name = "Precision", Price = 3000, Quantity = 5, CategoryId = 3, Description = "Smooth", BrandId = 2,Photo= @"Piano\piano1" },
                new Product() { Id = 3, Name = "Vintera", Price = 4000, Quantity = 2, CategoryId = 3, Description = "Blue bas", BrandId = 2,Photo= @"Piano\piano2" },
                new Product() { Id = 4, Name = "Epiphone", Price = 4000, Quantity = 2, CategoryId = 3, Description = "Advanced", BrandId = 1,Photo= @"Piano\piano3" },
                new Product() { Id = 5, Name = "Youngster", Price = 1100, Quantity = 8, CategoryId = 2, Description = "For kids", BrandId = 5, Photo = @"Bas\bas1" },
                new Product() { Id = 6, Name = "MPS-150X", Price = 3200, Quantity = 4, CategoryId = 2, Description = "For good players", BrandId = 5, Photo = @"Bas\bas2" },
                new Product() { Id = 7, Name = "DTX­432K", Price = 5600, Quantity = 2, CategoryId = 1, Description = "Nice set of drums", BrandId = 3, Photo = @"Drum set\drum1" },
                new Product() { Id = 8, Name = "P116M", Price = 8000, Quantity = 1, CategoryId = 4, Description = "Black and black", BrandId = 3, Photo = @"Keyboard\keyboard1" },
                new Product() { Id = 9, Name = "Calvinova", Price = 8900, Quantity = 1, CategoryId = 4, Description = "Old model", BrandId = 3, Photo = @"Keyboard\keyboard2" },
                new Product() { Id = 10, Name = "B2SP", Price = 2300, Quantity = 6, CategoryId = 3, Description = "Digitalpiano", BrandId = 4, Photo = @"Piano\piano4" },
                new Product() { Id = 11, Name = "SP-280", Price = 5300, Quantity = 3, CategoryId = 5, Description = "Traveling model", BrandId = 4, Photo = @"Guitar\guitar2" },
                new Product() { Id = 12, Name = "P-45", Price = 4900, Quantity = 3, CategoryId = 4, Description = "Our best keyboard", BrandId = 3, Photo = @"Keyboard\keyboard3" }


                ); 




        }
        


    }

}
