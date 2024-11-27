using AutoMapper;
using TelegramBot.App.DTO;
using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Services;

public class AutoService : IAutoService
{
    IAutoStorage _autoStorage;
    private IMapper _mapper;
    public AutoService(IAutoStorage autoStorage, IMapper mapper)
    {
        _autoStorage = autoStorage;
        _mapper = mapper;
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

    public async Task UpdateAutoAsync(Auto auto)
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