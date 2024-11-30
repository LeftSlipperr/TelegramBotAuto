namespace TelegramBot.Domain.Models;

public class Person
{
    public Guid Id { get; set; }
    
    public long chatId { get; set; }
    public string Name{ get; set; }
    public string SecondName { get; set; }
    public string ThirdName { get; set; }
    public string UserName {get; set;}
    public string PhoneNumber { get; set; }
    public bool AddingAuto { get; set; }
    public string? TempAutoData { get; set; }
    public int RegistrationStep {get; set;}
    
    public ICollection<Auto> Autos { get; set; }
}