using GasMonitoringApi.Auth;
using GasMonitoringApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Well> Wells => Set<Well>();
    public DbSet<Measurement> Measurements => Set<Measurement>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Well>(entity =>
        {
            entity.Property(w => w.Name).HasMaxLength(100).IsRequired();
            entity.ToTable("Wells", table =>
            {
                table.HasCheckConstraint("CK_Wells_Name", "length(trim(Name)) BETWEEN 1 AND 100");
                table.HasCheckConstraint("CK_Wells_MaxPressure", "MaxPressure BETWEEN 0.01 AND 1000");
                table.HasCheckConstraint("CK_Wells_PlannedFlow", "PlannedFlow BETWEEN 0.01 AND 1000000");
            });
        });

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasOne(m => m.Well)
                .WithMany(w => w.Measurements)
                .HasForeignKey(m => m.WellId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(m => m.RecordedAt).HasConversion(
                value => value.UtcDateTime.Ticks,
                value => new DateTimeOffset(value, TimeSpan.Zero));
            entity.HasIndex(m => new { m.WellId, m.RecordedAt });
            entity.ToTable("Measurements", table =>
            {
                table.HasCheckConstraint("CK_Measurements_Pressure", "Pressure BETWEEN 0 AND 1000");
                table.HasCheckConstraint("CK_Measurements_Flow", "Flow BETWEEN 0 AND 1000000");
            });
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.Login).HasMaxLength(40).IsRequired();
            entity.Property(u => u.NormalizedLogin).HasMaxLength(40).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(10).IsRequired();
            entity.HasIndex(u => u.NormalizedLogin).IsUnique();
            entity.ToTable("Users", table =>
                table.HasCheckConstraint("CK_Users_Role", "Role IN ('User', 'Admin')"));
        });
    }
}
