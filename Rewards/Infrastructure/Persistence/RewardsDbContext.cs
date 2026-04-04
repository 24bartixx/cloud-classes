using Microsoft.EntityFrameworkCore;

namespace Rewards.Service.Infrastructure.Persistence;

public class RewardsDbContext : DbContext
{
    public RewardsDbContext(DbContextOptions<RewardsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // This ensures all tables for this service use the dedicated schema
        modelBuilder.HasDefaultSchema("RewardsSchema");
    }
}
