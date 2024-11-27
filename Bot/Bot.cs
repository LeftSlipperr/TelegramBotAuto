using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot;

public class Program
{
    // Клиент для работы с Telegram Bot API
    private static ITelegramBotClient _botClient;

    // Настройки работы бота
    private static ReceiverOptions _receiverOptions;

    public static async Task Main(string[] args) // Указаны параметры для запуска из консоли
    {
        _botClient = new TelegramBotClient("7927427975:AAE1Kmiqn94Ae6wEb0eXcsRJec-874LYLxU"); // Укажите реальный токен
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message, // Разрешаем получать только сообщения
            },
        };

        using var cts = new CancellationTokenSource();

        // Запускаем бота
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); // Бесконечное ожидание для работы бота
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            // Обрабатываем входящие обновления
            switch (update.Type)
            {
                case UpdateType.Message:
                    Console.WriteLine("Пришло сообщение!");
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var errorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
