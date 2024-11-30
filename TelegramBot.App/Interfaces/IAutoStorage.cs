using TelegramBot.Domain.Models;

namespace TelegramBot.App.Interfaces;

public interface IAutoStorage
{
    Task AutoAddAsync(Auto auto);
    Task AutoDeleteAsync(Auto auto);
    Task<List<Auto>> GetAutosAsync(Guid personId);
    Task<Auto> GetAutoAsync(Guid autoId);
    Task UpdateAutoAsync(Auto auto);
    Task<List<Auto>> GetAutosByParametersAsync(string? Brand = null, string? ImageUrl = null,
        int? YearofIssue = null, string? Body = null, int? SeatInTheCabin = null,
        long? chatId = null, int? EngineSize = null, string? Transmission = null,  string? Drive = null,
        int? Mileage = null, string? Registration = null, Guid? PersonId = null,
        int pageNumber = 1, int pageSize = 10, string sortBy = "Brand");

    Task<List<Auto>> GetBrandByParametersAsync(string? Brand = null,
        int pageNumber = 1, int pageSize = 10, string sortBy = "Brand");
}