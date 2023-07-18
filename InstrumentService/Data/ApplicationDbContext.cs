﻿using InstrumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }

        public DbSet<Product> Product { get; set; }
    }
}
