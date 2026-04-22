using Clans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clans.Service.Infrastructure.Persistence;

public class ClansDbContext : DbContext
{
    public ClansDbContext(DbContextOptions<ClansDbContext> options) : base(options)
    {
    }

    public DbSet<Clan> Clans { get; set; }
    public DbSet<ClanWarResult> ClanWarResults { get; set; }
    public DbSet<ClanResult> ClanResults { get; set; }
    public DbSet<FileMetadata> FileMetadata { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // This ensures all tables for this service use the dedicated schema
        modelBuilder.HasDefaultSchema("ClansSchema");

        modelBuilder.Entity<Clan>(entity =>
        {
            entity.ToTable("Clans");
            entity.HasKey(e => e.ClanId);
        });

        modelBuilder.Entity<ClanWarResult>(entity =>
        {
            entity.ToTable("ClanWarResults");
            entity.HasKey(e => e.ClanWarId);
        });

        modelBuilder.Entity<ClanResult>(entity =>
        {
            entity.ToTable("ClansResults");
            entity.HasKey(e => e.ClanResultId);
        });

        modelBuilder.Entity<FileMetadata>(entity =>
        {
            entity.ToTable("FileMetadata");
            entity.HasKey(e => e.FileMetadataId);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FileExtension).HasMaxLength(32).IsRequired();
            entity.Property(e => e.S3BucketName).HasMaxLength(128).IsRequired();
            entity.Property(e => e.S3ObjectKey).HasMaxLength(1024).IsRequired();
        });
    }
}
