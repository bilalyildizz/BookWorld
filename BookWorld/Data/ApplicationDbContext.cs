using BookWorld.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWorld.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Publisher> Publisher { get; set; }
        public DbSet<Subcategory> Subcategory { get; set; }
        public DbSet<Translator> Translator { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



    }
}
