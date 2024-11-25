using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IPersonStorage
{
    Task AddPersonAsync(Person person);
    Task EditPersonAsync(Person person);
    Task DeletePersonAsync(Person person);
    Task<Person> GetPersonsAsync(Guid personId);
    
}