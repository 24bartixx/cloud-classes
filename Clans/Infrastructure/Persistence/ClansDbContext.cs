using Microsoft.EntityFrameworkCore;

namespace Clans.Service.Infrastructure.Persistence;

public class ClansDbContext : DbContext
{
    public ClansDbContext(DbContextOptions<ClansDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // This ensures all tables for this service use the dedicated schema
        modelBuilder.HasDefaultSchema("ClansSchema");
    }
}
