using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TelegramBot.App.Interfaces;
using TelegramBot.App.Services;
using TelegramBot.App.Validators;
using TelegramBot.Data;
using TelegramBot.Data.Storage;

var builder = WebApplication.CreateBuilder(args);


/*var connectionString = builder.Configuration.GetConnectionString("TelegramBotDb");
builder.Services.AddDbContext<TelegramBotDbContext>(options =>
    options.UseNpgsql(connectionString));*/


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IAutoService, AutoService>();
builder.Services.AddScoped<IPersonStorage, PersonStorage>();
builder.Services.AddScoped<IAutoStorage, AutoStorage>();

builder.Services.AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<PersonDtoValidator>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();