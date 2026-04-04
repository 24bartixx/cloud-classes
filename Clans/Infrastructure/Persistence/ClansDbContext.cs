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

        var clans = Enumerable.Range(1, 10).Select(i => new Clan
        {
            ClanId = Guid.Parse($"00000000-0000-0000-0000-{i:D12}"),
            ClanName = $"Clan {i}",
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        }).ToArray();

        var wars = Enumerable.Range(1, 70).Select(i => new ClanWarResult
        {
            ClanWarId = Guid.Parse($"10000000-0000-0000-0000-{i:D12}"),
            FinishDate = DateTime.UtcNow.AddHours(-i),
            TotalClans = (i % 3) + 2
        }).ToArray();

        var random = new Random(42); 
        var results = Enumerable.Range(1, 300).Select(i => new ClanResult
        {
            ClanResultId = Guid.Parse($"20000000-0000-0000-0000-{i:D12}"),
            ClanId = clans[random.Next(clans.Length)].ClanId,
            ClanWarId = wars[random.Next(wars.Length)].ClanWarId,
            Placement = (i % 4) + 1,
            Score = random.Next(1000, 5000)
        }).ToArray();

        modelBuilder.Entity<Clan>().HasData(clans);
        modelBuilder.Entity<ClanWarResult>().HasData(wars);
        modelBuilder.Entity<ClanResult>().HasData(results);
    }
}
