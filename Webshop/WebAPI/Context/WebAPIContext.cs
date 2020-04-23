﻿//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Context
{
    public class WebAPIContext : IdentityDbContext<User, AppRole, int>
    {
        public WebAPIContext()
        {
            // ...
        }

        public WebAPIContext(DbContextOptions<WebAPIContext> options) : base(options)
        {
            // ...
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductOrder>(entity =>
            {
                entity.Property(x => x.Discount).HasDefaultValue(0).HasColumnType("decimal(3,2)");
                entity.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(x => x.Amount).IsRequired();
                entity.Property(x => x.OrderId).IsRequired();
                entity.Property(x => x.ProductId).IsRequired();

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
                entity.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(x => x.Quantity).IsRequired();
                entity.Property(x => x.FullDescription).HasMaxLength(10000);
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

            builder.Entity<ShoppingCart>(entity =>
            {
                entity.Property(x => x.CartId).IsRequired();
                entity.Property(x => x.ProductId).IsRequired();
                entity.Property(x => x.Amount).IsRequired();
            });

            builder.Entity<News>(entity =>
            {
                entity.Property(x => x.Title).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Text).IsRequired();
                entity.Property(x => x.NewsDate).IsRequired();
            });

            builder.Entity<Rating>(entity =>
            {
                entity.Property(x => x.UserId).IsRequired();
                entity.Property(x => x.ProductId).IsRequired();
                entity.Property(x => x.RateDate).IsRequired();
                entity.Property(x => x.Score).IsRequired();
                entity.Property(x => x.Comment).IsRequired().HasMaxLength(200);
            });


            builder.Entity<News>().HasData(
                new News() {
                    Id = 1,
                    Title = "Butiken växer",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.",
                    NewsDate = DateTime.Now.AddDays(-8)
                }
            );

            builder.Entity<News>().HasData(
                new News()
                {
                    Id = 2,
                    Title = "Nya Produkter",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.",
                    NewsDate = DateTime.Now.AddDays(-6)
                }
            );

            builder.Entity<News>().HasData(
                new News()
                {
                    Id = 3,
                    Title = "Pressade Priser",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.",
                    NewsDate = DateTime.Now.AddDays(-5)
                }
            );

            builder.Entity<News>().HasData(
                new News()
                {
                    Id = 4,
                    Title = "Som en käftsmäll",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.",
                    NewsDate = DateTime.Now.AddDays(-3)
                }
            );

            builder.Entity<News>().HasData(
                new News()
                {
                    Id = 5,
                    Title = "Vi provar RG2750",
                    Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam mollis eu arcu at rhoncus. Cras ut felis dui. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aenean vitae aliquet dui. Suspendisse fermentum risus ut arcu condimentum, nec fringilla turpis mattis.",
                    NewsDate = DateTime.Now.AddDays(-1)
                }
            );

            builder.Entity<Brand>().HasData(
                new Brand() { Id = 1, Name = "Gibson" },
                new Brand() { Id = 2, Name = "Fender" },
                new Brand() { Id = 3, Name = "Yamaha" },
                new Brand() { Id = 4, Name = "Korg" },
                new Brand() { Id = 5, Name = "Millenium" }
            );

            builder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Trummor" },
                new Category() { Id = 2, Name = "Bas" },
                new Category() { Id = 3, Name = "Piano" },
                new Category() { Id = 4, Name = "Keyboard" },
                new Category() { Id = 5, Name = "Gitarr" }
            );

            builder.Entity<Product>().HasData(
                new Product() { Id = 1, Name = "Stratocaster", Price = 4000, Quantity = 4, CategoryId = 5, Description = "Black and white", BrandId = 2, Photo = @"Guitar/guitar1_original.jpg", ActiveProduct = true },
                new Product() { Id = 2, Name = "Precision", Price = 3000, Quantity = 5, CategoryId = 3, Description = "Smooth", BrandId = 2, Photo = @"Piano/piano1_original.jpg", ActiveProduct = true },
                new Product() { Id = 3, Name = "Vintera", Price = 4000, Quantity = 2, CategoryId = 3, Description = "Blue bas", BrandId = 2, Photo = @"Piano/piano2_original.jpg", ActiveProduct = true },
                new Product() { Id = 4, Name = "Epiphone", Price = 4000, Quantity = 2, CategoryId = 3, Description = "Advanced", BrandId = 1, Photo = @"Piano/piano3_original.jpg", ActiveProduct = true },
                new Product() { Id = 5, Name = "Youngster", Price = 1100, Quantity = 8, CategoryId = 2, Description = "For kids", BrandId = 5, Photo = @"Bas/bas1_original.jpg", ActiveProduct = true },
                new Product() { Id = 6, Name = "MPS-150X", Price = 3200, Quantity = 4, Discount = 0.1f, CategoryId = 2, Description = "For good players", BrandId = 5, Photo = @"Bas/bas2_original.jpg", ActiveProduct = true },
                new Product() { Id = 7, Name = "DTX­432K", Price = 5600, Quantity = 2, Discount = 0.25f, CategoryId = 1, Description = "Nice set of drums", BrandId = 3, Photo = @"Drum set/drum1_original.jpg", ActiveProduct = true },
                new Product() { Id = 8, Name = "P116M", Price = 8000, Quantity = 1, Discount = 0.3f, CategoryId = 4, Description = "Black and black", BrandId = 3, Photo = @"Keyboard/keyboard1_original.jpg", ActiveProduct = true },
                new Product() { Id = 9, Name = "Calvinova", Price = 8900, Quantity = 1, CategoryId = 4, Description = "Old model", BrandId = 3, Photo = @"Keyboard/keyboard2_original.jpg", ActiveProduct = true },
                new Product() { Id = 10, Name = "B2SP", Price = 2300, Quantity = 6, CategoryId = 3, Description = "Digitalpiano", BrandId = 4, Photo = @"Piano/piano4_original.jpg", ActiveProduct = true },
                new Product() { Id = 11, Name = "SP-280", Price = 5300, Quantity = 3, CategoryId = 5, Description = "Traveling model", BrandId = 4, Photo = @"Guitar/guitar2_original.jpg", ActiveProduct = true },
                new Product() { Id = 12, Name = "P-45", Price = 4900, Quantity = 3, CategoryId = 4, Description = "Our best keyboard", BrandId = 3, Photo = @"Keyboard/keyboard3_original.jpg" }


                );
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod() { Id = 1, Name = "Kort" },
                new PaymentMethod() { Id = 2, Name = "Swish" },
                new PaymentMethod() { Id = 3, Name = "Faktura" },
                new PaymentMethod() { Id = 4, Name = "Toapapper" }

                );
            builder.Entity<Status>().HasData(
                new Status() { Id = 1, Name = "Under behandling" },
                new Status() { Id = 2, Name = "Packas" },
                new Status() { Id = 3, Name = "Skickade" },
                new Status() { Id = 4, Name = "Levereras" }
                );


        }

    }
}
