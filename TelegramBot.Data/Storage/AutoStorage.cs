using Microsoft.EntityFrameworkCore;
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
    
    public async Task<List<Auto>> GetAutosByParametersAsync(string? Brand = null, string? ImageUrl = null,
        int? YearofIssue = null, string? Body = null, int? SeatInTheCabin = null,
        long? chatId = null, int? EngineSize = null, string? Transmission = null,  string? Drive = null,
        int? Mileage = null, string? Registration = null, Guid? PersonId = null,
        int pageNumber = 1, int pageSize = 10, string sortBy = "Brand")
    {
        var query = _dbContext.Autos.AsQueryable();
            
        if (!string.IsNullOrEmpty(Brand)) query = query.Where(c => c.Brand.Contains(Brand));
        if (!string.IsNullOrEmpty(ImageUrl)) query = query.Where(c => c.ImageUrl.Contains(ImageUrl));
        if (!string.IsNullOrEmpty(Body)) query = query.Where(c => c.Body.Contains(Body));
        if (!string.IsNullOrEmpty(Transmission)) query = query.Where(c => c.Transmission.Contains(Transmission));
        if (!string.IsNullOrEmpty(Drive)) query = query.Where(c => c.Drive.Contains(Drive));
        if (!string.IsNullOrEmpty(Registration)) query = query.Where(c => c.Registration.Contains(Registration));
        if (chatId != null) query = query.Where(c => c.chatId == chatId.Value);
        if (YearofIssue != null) query = query.Where(c => c.YearofIssue == YearofIssue.Value);
        if (SeatInTheCabin != null) query = query.Where(c => c.SeatInTheCabin == SeatInTheCabin.Value);
        if (EngineSize != null) query = query.Where(c => c.EngineSize == EngineSize.Value);
        if (Mileage != null) query = query.Where(c => c.Mileage == Mileage.Value);
        if (PersonId != null) query = query.Where(c => c.PersonId == PersonId.Value);

        if (sortBy == "Brand")
        {
            query = query.OrderBy(c => c.Brand);
        }
        else if (sortBy == "ImageUrl")
        {
            query = query.OrderBy(c => c.ImageUrl);
        }
        else if (sortBy == "Body")
        {
            query = query.OrderBy(c => c.Body);
        }
        else if (sortBy == "Transmission")
        {
            query = query.OrderBy(c => c.Transmission);
        }
        else if (sortBy == "Drive")
        {
            query = query.OrderBy(c => c.Drive);
        }
        else if (sortBy == "Registration")
        {
            query = query.OrderBy(c => c.Registration);
        }
        else if (sortBy == "chatId")
        {
            query = query.OrderBy(c => c.chatId);
        }
        else if (sortBy == "YearofIssue")
        {
            query = query.OrderBy(c => c.YearofIssue);
        }
        else if (sortBy == "EngineSize")
        {
            query = query.OrderBy(c => c.EngineSize);
        }
        else if (sortBy == "Mileage")
        {
            query = query.OrderBy(c => c.Mileage);
        }
            
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            
        return await query.ToListAsync();
    }
}