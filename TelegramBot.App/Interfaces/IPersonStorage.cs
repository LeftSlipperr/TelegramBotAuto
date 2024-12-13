using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IPersonStorage
{
    Task AddPersonAsync(Person person);
    Task EditPersonAsync(Person person);
    Task DeletePersonAsync(Guid personId);
    Task<Person> GetPersonsAsync(Guid personId);

    Task<List<Person>> GetPersonsByParametersAsync(string? Name = null, string? SecondName = null,
        string? ThirdName = null, string? PhoneNumber = null, string? UserName = null,
        long? chatId = null, int pageNumber = 1, int pageSize = 10, string sortBy = "Name");

}