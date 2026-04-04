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

        // Seed initial records
        var random = new Random(77);
        var tankIds = new[] { "T-34", "Tiger I", "M4 Sherman", "IS-2", "Panzer IV", null };

        var rewards = Enumerable.Range(1, 40).Select(i => new Reward
        {
            RewardId = Guid.Parse($"30000000-0000-0000-0000-{i:D12}"),
            MinKills = (i % 5 == 0) ? null : random.Next(1, 10),
            MaxDeaths = (i % 7 == 0) ? null : random.Next(0, 5),
            MinDamageDealt = (i % 3 == 0) ? null : random.Next(500, 5000),
            MaxDamageReceived = (i % 4 == 0) ? null : random.Next(100, 2000),
            MinSurvived = (i % 6 == 0) ? null : 1,
            MinExperienceEarned = (i % 2 == 0) ? null : random.Next(100, 1000),
            TankId = tankIds[random.Next(tankIds.Length)],
            Experience = random.Next(10, 200) * 5,
            Credits = random.Next(100, 5000) * 10
        }).ToArray();

        modelBuilder.Entity<Reward>().HasData(rewards);
    }
}
