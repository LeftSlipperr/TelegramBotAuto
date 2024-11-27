using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data.Storage;

public class PersonStorage : IPersonStorage
{
    private readonly TelegramBotDbContext _dbContext;

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
}