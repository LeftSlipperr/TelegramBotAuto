using TelegramBot.App.DTO;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IAutoService
{
    Task AutoAddAsync(Auto auto);
    Task AutoDeleteAsync(Auto auto);
    Task<List<Auto>> GetAutosAsync(Guid personId);
    Task<Auto> GetAutoAsync(Guid auto);
    Task UpdateAutoAsync(Auto auto);
}