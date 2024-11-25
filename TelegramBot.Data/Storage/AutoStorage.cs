using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data.Storage;

public class AutoStorage : IAutoStorage
{
    private readonly TelegramBotDbContext _dbContext;

    public AutoStorage(TelegramBotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AutoAddAsync(Auto auto)
    {
        await _dbContext.Autos.AddAsync(auto);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AutoDeleteAsync(Auto auto)
    {
        _dbContext.Autos.Remove(auto);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Auto>> GetAutosAsync(Guid personId)
    {
        await _dbContext.Autos.FindAsync(personId);
        return _dbContext.Autos.ToList();
    }

    public async Task<Auto> GetAutoAsync(Guid autoId)
    {
        return await _dbContext.Autos.FindAsync(autoId);
    }

    public async Task UpdateAutoAsync(Auto auto)
    {
        _dbContext.Autos.Update(auto);
        await _dbContext.SaveChangesAsync();
    }
}