namespace TelegramBot.Domain.Models;

public class Auto
{
    public Guid Id { get; set; }
    public string Brand { get; set; }
    public string YearofIssue { get; set; }
    public string Body { get; set; }
    public string SeatInTheCabin  { get; set; }
    public string FuelType { get; set; }
    public string EngineSize { get; set; }
    public string Transmission  { get; set; }
    public string Drive {get; set;}
    public int Mileage {get; set;}
    public string Registration {get; set;}
    public Guid ClientId {get; set;}
}