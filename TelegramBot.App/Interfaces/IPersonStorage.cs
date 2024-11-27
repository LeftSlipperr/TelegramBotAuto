using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IPersonStorage
{
    Task AddPersonAsync(Person person);
    Task EditPersonAsync(Person person);
    Task DeletePersonAsync(Guid personId);
    Task<Person> GetPersonsAsync(Guid personId);
    
}