using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data;

public class TelegramBotDbContext : DbContext
{
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Auto> Autos => Set<Auto>();
    
    public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}