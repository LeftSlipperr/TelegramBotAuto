using AutoMapper;
using TelegramBot.App.DTO;
using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Services;

public class PersonService : IPersonService
{
    IPersonStorage _personStorage;
    private IMapper _mapper;

    public PersonService(IPersonStorage personStorage, IMapper mapper)
    {
        _personStorage = personStorage;
        _mapper = mapper;
    }

    public async Task AddPersonAsync(PersonDto personDto)
    {
        if(personDto == null)
            throw new ArgumentNullException(nameof(personDto));
        
        Person person = _mapper.Map<Person>(personDto);
        await _personStorage.AddPersonAsync(person);
    }

    public async Task EditPersonAsync(PersonDto personDto)
    {
        if(personDto == null)
            throw new ArgumentNullException(nameof(personDto));
        
        Person person = _mapper.Map<Person>(personDto);
        await _personStorage.EditPersonAsync(person);
    }

    public async Task DeletePersonAsync(Guid personId)
    {
        if(personId == Guid.Empty)
            throw new ArgumentNullException(nameof(personId));
        
        await _personStorage.DeletePersonAsync(personId);
    }

    public async Task<Person> GetPersonsAsync(Guid personId)
    {
        if(personId == Guid.Empty)
            throw new ArgumentNullException(nameof(personId));
        
        return await _personStorage.GetPersonsAsync(personId);
    }
}