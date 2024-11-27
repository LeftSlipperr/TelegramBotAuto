using Microsoft.AspNetCore.Mvc;
using TelegramBot.App.DTO;
using TelegramBot.App.Interfaces;
using TelegramBot.Domain.Models;

namespace TelegramBot.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private IPersonService _personService;
    
    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await _personService.GetPersonsAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task AddPersonAsync([FromBody] PersonDto personDto)
    {
        await _personService.AddPersonAsync(personDto);
    }

    [HttpPut]
    public async Task UpdatePersonAsync([FromBody] PersonDto personDto)
    {
        await _personService.AddPersonAsync(personDto);
    }

    [HttpDelete("Person/{personId:guid}")]
    public async Task DeletePersonAsync([FromRoute] Guid personId)
    {
        await _personService.DeletePersonAsync(personId);
    }
}