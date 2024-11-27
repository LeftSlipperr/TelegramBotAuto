using FluentValidation;
using TelegramBot.App.DTO;
using TelegramBot.Domain.Models;

namespace TelegramBot.App.Validators;

public class AutoValidator : AbstractValidator<Auto>
{
    public AutoValidator()
    {
        RuleFor(a => a.Brand)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите марку машины");

        RuleFor(a => a.YearofIssue)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите год выпуска");
        
        RuleFor(a => a.Body)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите вид кузова");

        RuleFor(a => a.SeatInTheCabin)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите количество сидений в машине");

        RuleFor(a => a.FuelType)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите вид топлива");
        
        RuleFor(a => a.EngineSize)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите объем двигателя");
        
        RuleFor(a => a.Transmission)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите коробку передач");

        RuleFor(a => a.Drive)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите привод");

        RuleFor(a => a.Mileage)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите пробег");

        RuleFor(a => a.Registration)
            .NotNull()
            .NotEmpty()
            .WithMessage("Укажите регистрацию");

        

        
    }
}