using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FranklinMutualAPI.Models
{
    public partial class FranklinMutualContext : DbContext
    {
        public virtual DbSet<Agency> Agency { get; set; }

        public FranklinMutualContext(DbContextOptions<FranklinMutualContext> dbContextOptions)
            : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>(entity =>
            {
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
        }
    }
}
