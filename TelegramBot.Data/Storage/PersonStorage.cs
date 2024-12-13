using Microsoft.EntityFrameworkCore;
using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data.Storage;

public class PersonStorage : IPersonStorage
{
    private static TelegramBotDbContext _dbContext;

    public PersonStorage(TelegramBotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddPersonAsync(Person person)
    {
        await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditPersonAsync(Person person)
    {
        _dbContext.Persons.Update(person);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePersonAsync(Guid personId)
    {
        _dbContext.Persons.Remove(await _dbContext.Persons.FindAsync(personId));
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Person> GetPersonsAsync(Guid personId)
    {
        return await _dbContext.Persons.FindAsync(personId);
    }
    
    public  async Task<List<Person>> GetPersonsByParametersAsync(string? Name = null, string? SecondName = null, string? ThirdName = null, string? PhoneNumber = null, string? UserName = null,
        long? chatId = null, int pageNumber = 1, int pageSize = 10, string sortBy = "Name")
    {
        var query = _dbContext.Persons.AsQueryable();
            
        if (!string.IsNullOrEmpty(Name)) query = query.Where(c => c.Name.Contains(Name));
        if (!string.IsNullOrEmpty(SecondName)) query = query.Where(c => c.SecondName.Contains(SecondName));
        if (!string.IsNullOrEmpty(ThirdName)) query = query.Where(c => c.ThirdName.Contains(ThirdName));
        if (!string.IsNullOrEmpty(PhoneNumber)) query = query.Where(c => c.PhoneNumber.Contains(PhoneNumber));
        if (!string.IsNullOrEmpty(UserName)) query = query.Where(c => c.PhoneNumber.Contains(PhoneNumber));
        if (chatId != null) query = query.Where(c => c.chatId == chatId.Value);
            
        if (sortBy == "Name")
        {
            query = query.OrderBy(c => c.Name);
        }
        else if (sortBy == "Age")
        {
            query = query.OrderBy(c => c.chatId);
        }
            
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            
        return await query.ToListAsync();
    }
}