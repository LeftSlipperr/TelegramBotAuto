namespace TelegramBot.Domain.Models;

public class Person
{
    public Guid Id { get; set; }
    public string Name{ get; set; }
    public string SecondName { get; set; }
    public string ThirdName { get; set; }
    public string UserName {get; set;}
    public int Age { get; set; }
}