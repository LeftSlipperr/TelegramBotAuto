namespace TelegramBot.Domain.Models;

public class Auto
{
    public Guid Id { get; set; }
    public Guid PersonId {get; set;}
    public long chatId { get; set; }
    public string Brand { get; set; }
    public string ImageUrl { get; set; }
    public int YearofIssue { get; set; }
    public string Body { get; set; }
    public int SeatInTheCabin  { get; set; }
    public string FuelType { get; set; }
    public double EngineSize { get; set; }
    public string Transmission  { get; set; }
    public string Drive {get; set;}
    public int Mileage {get; set;}
    public string Registration {get; set;}
}