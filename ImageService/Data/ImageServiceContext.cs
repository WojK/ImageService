using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using ImagesService.Models;

namespace ImagesService.Data
{
    public class ImageServiceContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Models.Image> Images { get; set; }
        public DbSet<SelectedUsers> SelectedUsers { get; set; }


        readonly string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string db = "imagaServiceDb.db";

        public ImageServiceContext()
        {
            dbPath = Path.Combine(dbPath, "Databases", db);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={dbPath}").UseLazyLoadingProxies();
        }
    }
}
