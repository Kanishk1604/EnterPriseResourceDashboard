using Erd.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Erd.Api.Data;

//how .Net interacts with the database

public class AppDbContext : DbContext   //AppDbContext inherits from DbContext
{
    //DbContextOptions contains connection string, SQL server provider, EF configurations
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } //base(options) passes options to DbContext constructor

    //creating dabase tables where each row maps to an entity
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Manager" },
            new Role { Id = 3, Name = "Employee" }
        );

        base.OnModelCreating(modelBuilder);
    }
}