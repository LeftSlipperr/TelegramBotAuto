using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IAutoStorage
{
    Task AutoAddAsync(Auto auto);
    Task AutoDeleteAsync(Auto auto);
    Task<List<Auto>> GetAutosAsync(Guid personId);
    Task<Auto> GetAutoAsync(Guid autoId);
    Task UpdateAutoAsync(Auto auto);
}