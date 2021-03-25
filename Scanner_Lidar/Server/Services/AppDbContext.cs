using Microsoft.EntityFrameworkCore;
using Server_Lidar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server_Lidar.Services
{
    
    public class AppDbContext : DbContext
    {
        private static string source = "c:\\temp\\Lidar.3AA.db";
        private static string  path = Path.Combine(AppContext.BaseDirectory, source);
        public DbSet<Vector3> Vector3s { get; set; }
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=" + path);
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
