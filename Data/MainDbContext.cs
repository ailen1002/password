using System;
using Microsoft.EntityFrameworkCore;
using password.Models;

namespace password.Data;

public class MainDbContext : DbContext
{
    public DbSet<AccountInfo> AccountInfo { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=password.db");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountInfo>()
            .Property(a => a.CreationDate)
            .HasDefaultValueSql("datetime('now', 'localtime')");
        modelBuilder.Entity<AccountInfo>()
            .Property(a => a.LastUpdated)
            .HasDefaultValueSql("datetime('now', 'localtime')");
    }
    // 确保数据库和表已创建
    public void EnsureDatabaseCreated()
    {
        try
        {
            Console.WriteLine(this.Database.EnsureCreated() // 如果数据库或表不存在，则创建
                ? "Database and tables created."
                : "Database already exists.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ensuring database creation: {ex.Message}");
        }
    }
}