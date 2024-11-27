using TelegramBot.App.DTO;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IPersonService
{
    public Task AddPersonAsync(PersonDto personDto);
    public Task EditPersonAsync(PersonDto personDto);
    public Task DeletePersonAsync(Guid personId);
    public Task<Person> GetPersonsAsync(Guid personId);
}