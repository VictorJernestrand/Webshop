using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Context
{
    public class WebshopContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Status> Statuses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Webshop;Trusted_Connection=True;");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductOrder>(entity =>
            {
                entity.Property(x => x.Discount).HasDefaultValue(0);
            });
            builder.Entity<User>(entity =>
            {
                entity.Property(x => x.FirstName).IsRequired();
                entity.Property(x => x.LastName).IsRequired();
                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.PhoneNumber).IsRequired();
                entity.Property(x => x.StreetAddress).IsRequired();
                entity.Property(x => x.ZipCode).IsRequired();
                entity.Property(x => x.City).IsRequired();
                entity.Property(x => x.Password).IsRequired().HasMaxLength(64);
            });
            builder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.Price).IsRequired();
                entity.Property(x => x.Quantity).IsRequired();
            });
            builder.Entity<Category>(entity =>
            {
                entity.Property(x => x.Name).IsRequired();

            });
            builder.Entity<Brand>(entity =>
            {
                entity.Property(x => x.Name).IsRequired();

            });
            builder.Entity<PaymentMethod>(entity =>
            {
                entity.Property(x => x.Name).IsRequired();

            });
            builder.Entity<Brand>().HasData(
           new Brand() {Id=1, Name = "Gibson" },
           new Brand() { Id = 2, Name = "Fender" },
           new Brand() { Id = 3, Name = "Yamaha" },
           new Brand() { Id = 4, Name = "Korg" },
           new Brand() { Id = 5, Name = "Millenium" }
          );
            builder.Entity<Category>().HasData(
                new Category() { Id=1,Name = "Drum set" },
                 new Category() { Id = 2, Name = "Bas" },
                  new Category() { Id = 3, Name = "Piano" },
                   new Category() { Id = 4, Name = "Keyboard" }

                );
            



        }

    }

}
