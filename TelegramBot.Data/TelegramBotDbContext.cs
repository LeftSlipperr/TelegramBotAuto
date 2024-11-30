using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data;

public class TelegramBotDbContext : DbContext
{
    
    public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options) : base(options) { }
    
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Auto> Autos => Set<Auto>();
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5435;Username=postgres;Password=postgres;Database=postgres");
    }
    
}