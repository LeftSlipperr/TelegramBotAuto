using FluentValidation;
using TelegramBot.App.DTO;

namespace TelegramBot.App.Validators;

public class PersonDtoValidator : AbstractValidator<PersonDto>
{
    public PersonDtoValidator()
    {
        RuleFor(p => p.FullName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Имя пользователя обязательно.");
        
        RuleFor(p => p.Username)
            .NotNull().WithMessage("Имя пользователя не должно быть пустым")
            .NotEmpty().WithMessage("Имя пользователя не должно быть пустым")
            .Matches(@"^@").WithMessage("Имя пользователя должно начинаться с '@'");

        RuleFor(p => p.PhoneNumber)
            .NotNull()
            .NotEmpty(). WithMessage("Номер телефона не может быть пустым")
            .MaximumLength(9);
    }
}