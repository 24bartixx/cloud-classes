using Rewards.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Rewards.Service.Infrastructure.Persistence;

public class RewardsDbContext : DbContext
{
    public RewardsDbContext(DbContextOptions<RewardsDbContext> options) : base(options)
    {
    }

    public DbSet<Reward> Rewards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // This ensures all tables for this service use the dedicated schema
        modelBuilder.HasDefaultSchema("RewardsSchema");

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.ToTable("Rewards");
            entity.HasKey(e => e.RewardId);

            entity.Property(e => e.Experience)
                .HasDefaultValue(0);

            entity.Property(e => e.Credits)
                .HasDefaultValue(0);
        });
    }
}
