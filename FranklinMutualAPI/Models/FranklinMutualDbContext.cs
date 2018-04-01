using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FranklinMutualAPI.Models
{
    public partial class FranklinMutualDbContext : DbContext
    {
        public virtual DbSet<Agency> Agency { get; set; }
        public virtual DbSet<Blog> Blog { get; set; }

        public FranklinMutualDbContext(DbContextOptions<FranklinMutualDbContext> dbContextOptions)
            : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>(entity =>
            {
                entity.HasIndex(e => e.Latitude)
                    .HasName("agency_latitude");

                entity.HasIndex(e => e.Longitude)
                    .HasName("agency_longitude");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(200);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.County)
                    .HasColumnName("county")
                    .HasMaxLength(50);

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("nchar(10)");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasColumnType("nchar(2)");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(200);

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasColumnType("nchar(5)");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnName("author")
                    .HasMaxLength(50);

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasColumnName("category")
                    .HasMaxLength(100);

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image")
                    .HasMaxLength(200);

                entity.Property(e => e.Publishdate)
                    .HasColumnName("publishdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasMaxLength(1000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasMaxLength(200);
            });
        }
    }
}
