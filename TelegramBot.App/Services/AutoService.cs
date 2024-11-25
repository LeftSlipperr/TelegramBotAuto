using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Services;

public class AutoService
{
    IAutoStorage _autoStorage;

    public AutoService(IAutoStorage autoStorage)
    {
        if (autoStorage == null)
            throw new ArgumentNullException(nameof(autoStorage));
        
        _autoStorage = autoStorage;
    }

    public async Task AutoAddAsync(Auto auto)
    {
        if (auto == null)
            throw new ArgumentNullException(nameof(auto));
        
        await _autoStorage.AutoAddAsync(auto);
    }

    public async Task AutoDeleteAsync(Auto auto)
    {
        if (auto == null)
            throw new ArgumentNullException(nameof(auto));
        
        await _autoStorage.AutoDeleteAsync(auto);
    }

    public async Task AutoUpdateAsync(Auto auto)
    {
        if (auto == null)
            throw new ArgumentNullException(nameof(auto));
        
        await _autoStorage.UpdateAutoAsync(auto);
    }

    public async Task<Auto> GetAutoAsync(Guid autoId)
    {
        if (autoId == Guid.Empty)
            throw new ArgumentNullException(nameof(autoId));
        
        return await _autoStorage.GetAutoAsync(autoId);
    }

    public async Task<List<Auto>> GetAutosAsync(Guid personId)
    {
        if (personId == Guid.Empty)
            throw new ArgumentNullException(nameof(personId));
        
        return await _autoStorage.GetAutosAsync(personId);
    }
}