using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using ImpliciteTesterServer.Data;
using Microsoft.EntityFrameworkCore;
using Squads.Infrastructure.EntityTypeConfigurations;

namespace ImpliciteTesterServer.Infrastructure
{
    public class MyDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<ImageUpload> ImageUploads { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Fase> Fases { get; set; }
        public DbSet<FaseTypeImage> FaseTypeImages { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<FaceReader> FaceReaders { get; set; }


        public MyDbContext(DbContextOptions<MyDbContext> options)
    : base(options)
        { }

        public MyDbContext()
        { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=ImpliciteTester.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new CategoryTypeConfiguration().Configure(modelBuilder.Entity<Category>());
            new ImageUploadTypeConfiguration().Configure(modelBuilder.Entity<ImageUpload>());
            new ImageTypeConfiguration().Configure(modelBuilder.Entity<Image>());
            new TestTypeConfiguration().Configure(modelBuilder.Entity<Test>());
            new FaseTypeConfiguration().Configure(modelBuilder.Entity<Fase>());
            new FaseTypeImageConfiguration().Configure(modelBuilder.Entity<FaseTypeImage>());
            new ResultTypeConfiguration().Configure(modelBuilder.Entity<Result>());
            new FaceReaderTypeConfiguration().Configure(modelBuilder.Entity<FaceReader>());






        }
    }
}

