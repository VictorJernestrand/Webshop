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
                entity.Property(x => x.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(x => x.LastName).IsRequired().HasMaxLength(30);
                entity.Property(x => x.Email).IsRequired().HasMaxLength(50);
                entity.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(x => x.StreetAddress).IsRequired().HasMaxLength(50);
                entity.Property(x => x.ZipCode).IsRequired().HasMaxLength(5);
                entity.Property(x => x.City).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Password).IsRequired().HasMaxLength(64);
            });

            // Make users email unique for each user!
            builder.Entity<User>().HasIndex(x => x.Email).IsUnique();

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

            builder.Entity<Admin>(entity =>
            {
                entity.Property(x => x.UserName).IsRequired().HasMaxLength(30);
                entity.Property(x => x.Password).IsRequired();
            });
        }

    }

}
