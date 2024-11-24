using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Services;

public class PersonService
{
    IPersonStorage _personStorage;

    public PersonService(IPersonStorage personStorage)
    {
        _personStorage = personStorage;
    }

    public async Task AddPersonAsync(Person person)
    {
        if(person == null)
            throw new ArgumentNullException(nameof(person));
        
        await _personStorage.AddPersonAsync(person);
    }

    public async Task EditPersonAsync(Person person)
    {
        if(person == null)
            throw new ArgumentNullException(nameof(person));
        
        await _personStorage.EditPersonAsync(person);
    }

    public async Task DeletePersonAsync(Person person)
    {
        if(person == null)
            throw new ArgumentNullException(nameof(person));
        
        await _personStorage.DeletePersonAsync(person);
    }

    public async Task<Person> GetPersonsAsync(Guid personId)
    {
        if(personId == Guid.Empty)
            throw new ArgumentNullException(nameof(personId));
        
        return await _personStorage.GetPersonsAsync(personId);
    }
}