using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.App.Interfaces;
using TelegramBot.Data.Storage;
using TelegramBot.Domain.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBot.Data;


class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;
    private static IServiceProvider _serviceProvider;

    static async Task Main()
    {
       var serviceProvider = new ServiceCollection()
            .AddDbContext<TelegramBotDbContext>(options =>
                options.UseNpgsql(
                    "Host=localhost;Port=5435;Username=postgres;Password=postgres;Database=postgres")) // Укажите строку подключения
            .AddSingleton<IPersonStorage, PersonStorage>()
            .AddSingleton<IAutoStorage, AutoStorage>()// Добавляем хранилище
            .BuildServiceProvider();

        _serviceProvider = serviceProvider;
        _botClient =
            new TelegramBotClient(
                "7927427975:AAE1Kmiqn94Ae6wEb0eXcsRJec-874LYLxU"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates =
                new
                    [] // Тут указываем типы получаемых Updateов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
                    {
                        UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                        UpdateType.CallbackQuery 
                    },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Updateов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(HandleUpdateAsync, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }

    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            var callbackQuery = update.CallbackQuery;
            if (callbackQuery.Data.StartsWith("delete_auto_"))
            {
                var autoId = Guid.Parse(callbackQuery.Data.Replace("delete_auto_", ""));
                await HandleDeleteAutoAsync(botClient, callbackQuery.Message.Chat.Id, autoId, cancellationToken);
                return;
            }else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Некорректный идентификатор объявления.", cancellationToken: cancellationToken);
            }
        }
       

        var message = update.Message;
        var chatId = message.Chat.Id;

        using var scope = _serviceProvider.CreateScope();
        var personStorage = scope.ServiceProvider.GetRequiredService<IPersonStorage>();

        var users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            // Проверяем, является ли сообщение форматом регистрации
            if (await TryRegisterUserAsync(personStorage, chatId, message.Text, cancellationToken))
            {
                // Если регистрация успешна, показываем главное меню
                await ShowMainMenuAsync(botClient, chatId, cancellationToken);
            }
            else
            {
                // Если формат данных некорректен, просим пользователя ввести данные снова
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Добро пожаловать! Для продолжения заполните ваши данные:\n\nВведите их в формате:\nИмя Фамилия Отчество @Ник Телефон",
                    cancellationToken: cancellationToken
                );
            }

            return; // Завершаем обработку, чтобы не продолжать лишнюю проверку
        }
        

        if (update.Message.Type == MessageType.Photo)
        {
            Console.WriteLine("Фото получено!");
            var photo = update.Message.Photo.OrderByDescending(p => p.FileSize).FirstOrDefault();
            if (photo != null)
            {
                // Токен вашего бота
                var botToken = "7927427975:AAE1Kmiqn94Ae6wEb0eXcsRJec-874LYLxU";

                // Получаем файл (например, фото)
                var file = await botClient.GetFileAsync(message.Photo.Last().FileId);

                // Строим URL для скачивания файла
                var fileUrl = $"https://api.telegram.org/file/bot{botToken}/{file.FilePath}";
                await HandlePhotoAsync(botClient, update.Message.Chat.Id, fileUrl, cancellationToken);
            }
        }

        if (user.AddingAuto) // Проверяем, в процессе ли пользователь добавления авто
        {
            if (message.Text == "Отменить действие")
            {
                user.AddingAuto = false; // Сбрасываем флаг
                await personStorage.EditPersonAsync(user); // Обновляем состояние пользователя в базе данных
                await botClient.SendTextMessageAsync(chatId, "Действие отменено.", cancellationToken: cancellationToken);
                return; // Завершаем обработку
            }
            
            if (await TryAddAutoAsync(scope.ServiceProvider.GetRequiredService<IAutoStorage>(), chatId, message.Text,
                    user, cancellationToken))
            {
                user.AddingAuto = false; // Сбрасываем флаг
                // Обновляем состояние в базе данных
                await personStorage.EditPersonAsync(user);
            }
            else 
            {
                await botClient.SendTextMessageAsync(chatId, "Некорректный формат данных. Попробуйте снова.", cancellationToken: cancellationToken);
            }
            return; // Завершаем обработку, так как состояние обработано
        }
        // Если пользователь найден, обрабатываем команды
        switch (message.Text)
        {
            case "/start":
                await ShowMainMenuAsync(botClient, chatId, cancellationToken);
                break;

            case "Добавить объявление":
                await StartAddingAutoAsync(botClient, chatId, cancellationToken);
                users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
                var currentUser = users.FirstOrDefault();

                if (currentUser != null && currentUser.SearchingAutoByBrand)
                {
                    await SearchAutosByBrandAsync(botClient, chatId, message.Text, cancellationToken);
                    currentUser.SearchingAutoByBrand = false; // Сбрасываем флаг
                    await personStorage.EditPersonAsync(currentUser);
                }
                break;
            case "Мои объявления":
                await ShowUserAutosAsync(botClient, chatId, cancellationToken);
                users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
                currentUser = users.FirstOrDefault();

                if (currentUser != null && currentUser.SearchingAutoByBrand)
                {
                    await SearchAutosByBrandAsync(botClient, chatId, message.Text, cancellationToken);
                    currentUser.SearchingAutoByBrand = false; // Сбрасываем флаг
                    await personStorage.EditPersonAsync(currentUser);
                }
                break;
            case "Отменить действие":
                ShowMainMenuAsync(botClient, chatId, cancellationToken);
                users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
                currentUser = users.FirstOrDefault();

                if (currentUser != null && currentUser.SearchingAutoByBrand)
                {
                    await SearchAutosByBrandAsync(botClient, chatId, message.Text, cancellationToken);
                    currentUser.SearchingAutoByBrand = false; // Сбрасываем флаг
                    await personStorage.EditPersonAsync(currentUser);
                }
                break;
            case "Поиск объявлений":
                await RequestBrandForSearchAsync(botClient, chatId, cancellationToken);
                break;;
            default:
                users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
                currentUser = users.FirstOrDefault();

                if (currentUser != null && currentUser.SearchingAutoByBrand)
                {
                    await SearchAutosByBrandAsync(botClient, chatId, message.Text, cancellationToken);
                    currentUser.SearchingAutoByBrand = false; // Сбрасываем флаг
                    await personStorage.EditPersonAsync(currentUser);
                }
                break;
        }


    }
    
    private static async Task HandleDeleteAutoAsync(ITelegramBotClient botClient, long chatId, Guid autoId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var autoStorage = scope.ServiceProvider.GetRequiredService<IAutoStorage>();
        Auto auto = autoStorage.GetAutosByParametersAsync(AutoId: autoId).Result.FirstOrDefault();

       await autoStorage.AutoDeleteAsync(auto);
       
       
        if (autoStorage.GetAutosByParametersAsync(AutoId: autoId).Result.Count() == 0)
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Объявление успешно удалено.",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "Не удалось удалить объявление. Возможно, оно уже удалено.",
                cancellationToken: cancellationToken
            );
        }
    }

    private static async Task RequestBrandForSearchAsync(
        ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var personStorage = scope.ServiceProvider.GetRequiredService<IPersonStorage>();
        var users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
        var user = users.FirstOrDefault();

        if (user != null)
        {
            user.SearchingAutoByBrand = true; // Устанавливаем флаг поиска
            await personStorage.EditPersonAsync(user);
        }

        await botClient.SendTextMessageAsync(
            chatId,
            "Введите название бренда для поиска автомобилей:",
            cancellationToken: cancellationToken
        );
    }
    
    private static async Task SearchAutosByBrandAsync(
        ITelegramBotClient botClient,
        long chatId,
        string brand,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var autoStorage = scope.ServiceProvider.GetRequiredService<IAutoStorage>();
        var personStorage = scope.ServiceProvider.GetRequiredService<IPersonStorage>();

        // Получаем список автомобилей по бренду
        var autos = await autoStorage.GetBrandByParametersAsync(Brand: brand);
        
        if (!autos.Any())
        {
            await botClient.SendTextMessageAsync(
                chatId,
                $"Автомобили с брендом \"{brand}\" не найдены.",
                cancellationToken: cancellationToken
            );
            return;
        }

        foreach (var auto in autos)
        {
            var users = await personStorage.GetPersonsByParametersAsync(chatId: auto.chatId);
            Person user = users.FirstOrDefault();
            
            var message =$"🚗 *Бренд*: {auto.Brand}\n" +
                         $"📅 *Год выпуска*: {auto.YearofIssue}\n" +
                         $"🚙 *Тип кузова*: {auto.Body}\n" +
                         $"💺 *Мест*: {auto.SeatInTheCabin}\n" +
                         $"⛽ *Тип топлива*: {auto.FuelType}\n" +
                         $"🔧 *Объем двигателя*: {auto.EngineSize} л\n" +
                         $"⚙️ *Трансмиссия*: {auto.Transmission}\n" +
                         $"🔗 *Привод*: {auto.Drive}\n" +
                         $"📏 *Пробег*: {auto.Mileage} км\n" +
                         $"📋 *Регистрация*: {auto.Registration}\n" +
                         $"👤 *Владелец*: { user.Name } {user.SecondName} {user.ThirdName} {user.UserName}\n" +
                         $"📞 *Телефон*: {user.PhoneNumber}";

            await botClient.SendTextMessageAsync(
                chatId,
                message,
                cancellationToken: cancellationToken
            );

            using var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync(auto.ImageUrl);
            await botClient.SendPhotoAsync(
                chatId,
                photo: stream,
                cancellationToken: cancellationToken
            );
        }
    }

    
    private static async Task<bool> TryRegisterUserAsync(
        IPersonStorage personStorage,
        long chatId,
        string input,
        CancellationToken cancellationToken)
    {
        var parts = input.Split(' ', StringSplitOptions.TrimEntries);
        if (parts.Length != 5 || !parts[3].StartsWith("@"))
        {
            return false;
        }

        var person = new Person
        {
            Id = Guid.NewGuid(),
            chatId = chatId,
            Name = parts[0],
            SecondName = parts[1],
            ThirdName = parts[2],
            UserName = parts[3],
            PhoneNumber = Convert.ToString(parts[4]),
        };

        await personStorage.AddPersonAsync(person);
        return true;
    }
    
    private static async Task ShowUserAutosAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var autoStorage = scope.ServiceProvider.GetRequiredService<IAutoStorage>();

        var autos = await autoStorage.GetAutosByParametersAsync(chatId: chatId);

        if (!autos.Any())
        {
            await botClient.SendTextMessageAsync(
                chatId,
                "У вас пока нет добавленных объявлений.",
                cancellationToken: cancellationToken
            );
            return;
        }

        foreach (var auto in autos)
        {
            var message = $"🚗 *Бренд*: {auto.Brand}\n" +
                          $"📅 *Год выпуска*: {auto.YearofIssue}\n" +
                          $"🚙 *Тип кузова*: {auto.Body}\n" +
                          $"💺 *Мест*: {auto.SeatInTheCabin}\n" +
                          $"⛽ *Тип топлива*: {auto.FuelType}\n" +
                          $"🔧 *Объем двигателя*: {auto.EngineSize} л\n" +
                          $"⚙️ *Трансмиссия*: {auto.Transmission}\n" +
                          $"🔗 *Привод*: {auto.Drive}\n" +
                          $"📏 *Пробег*: {auto.Mileage} км\n" +
                          $"📋 *Регистрация*: {auto.Registration}";
            
            
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("🗑 Удалить это объявление", $"delete_auto_ {auto.Id}")
            });

            await botClient.SendTextMessageAsync(
                chatId,
                message,
                replyMarkup: inlineKeyboard,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );
            
            using var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync(auto.ImageUrl);
            await botClient.SendPhotoAsync(
                chatId,
                photo: stream,
                cancellationToken: cancellationToken
            );
            
            
        }
    }

    
    private static async Task StartAddingAutoAsync(
    ITelegramBotClient botClient,
    long chatId,
    CancellationToken cancellationToken)
{
    using var scope = _serviceProvider.CreateScope();
    var personStorage = scope.ServiceProvider.GetRequiredService<IPersonStorage>();
    var users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
    var user = users.FirstOrDefault();

    if (user != null)
    {
        user.AddingAuto = true;
        await personStorage.EditPersonAsync(user); // Сохраняем изменения в базе данных
    }

    await botClient.SendTextMessageAsync(
        chatId,
        "Введите данные об автомобиле в формате:\n\n" +
        "Бренд, Год выпуска, Тип кузова, Количество мест, " +
        "Тип топлива, Объем двигателя(через точку), Трансмиссия, Привод, Пробег, Регистрация",
        cancellationToken: cancellationToken
    );
}

private static async Task<bool> TryAddAutoAsync(
    IAutoStorage autoStorage,
    long chatId,
    string input,
    Person user,
    CancellationToken cancellationToken)
{
    var parts = input.Split(',', StringSplitOptions.TrimEntries);
    if (parts.Length != 10 || !int.TryParse(parts[1], out var year) || !int.TryParse(parts[3], out var seats) ||
        !double.TryParse(parts[5],NumberStyles.Any, CultureInfo.InvariantCulture, out var engineSize) || !int.TryParse(parts[8], out var mileage))
    {
        return false;
    }

    // Сохраняем автомобиль без фото
    var auto = new Auto
    {
        Id = Guid.NewGuid(),
        PersonId = user.Id,
        chatId = chatId,
        Brand = parts[0],
        ImageUrl = string.Empty, // Пока что не устанавливаем ссылку на фото
        YearofIssue = year,
        Body = parts[2],
        SeatInTheCabin = seats,
        FuelType = parts[4],
        EngineSize = engineSize,
        Transmission = parts[6],
        Drive = parts[7],
        Mileage = mileage,
        Registration = parts[9]
    };

    await autoStorage.AutoAddAsync(auto);

    // Запрос на фотографию
    await _botClient.SendTextMessageAsync(
        chatId,
        "Теперь отправьте фотографию вашего автомобиля, и мы добавим её в объявление.",
        cancellationToken: cancellationToken
    );

    return true;
}

// Метод для получения фотографии и сохранения её URL
    private static async Task HandlePhotoAsync(ITelegramBotClient botClient, long chatId, string photoUrl, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Сохраняем фото: {photoUrl}");
        using var scope = _serviceProvider.CreateScope();
        var autoStorage = scope.ServiceProvider.GetRequiredService<IAutoStorage>();
        var personStorage = scope.ServiceProvider.GetRequiredService<IPersonStorage>();

        // Получаем пользователя
        var users = await personStorage.GetPersonsByParametersAsync(chatId: chatId);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            await botClient.SendTextMessageAsync(chatId, "Не удалось найти вашего пользователя.", cancellationToken: cancellationToken);
            return;
        }

        // Получаем автомобиль пользователя
        var autos = await autoStorage.GetAutosByParametersAsync(PersonId: user.Id);
        foreach (var auto in autos)
        {
            if (auto.ImageUrl == "")
            {
                if (auto == null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Не удалось найти ваш автомобиль.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // Обновляем информацию о фотографии
                auto.ImageUrl = photoUrl;

                // Сохраняем изменения в базе данных
                await autoStorage.UpdateAutoAsync(auto);
            }
        }
        

        // Подтверждаем успешную загрузку фотографии
        await botClient.SendTextMessageAsync(chatId, "Фотография добавлена в ваше объявление!", cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId, "Ваше объявление добавлено!", cancellationToken: cancellationToken);
    }

    private static async Task ShowMainMenuAsync(
        ITelegramBotClient botClient,
        long chatId,
        CancellationToken cancellationToken)
    {
        var buttons = new[]
        {
            new KeyboardButton("Мои объявления"),
            new KeyboardButton("Добавить объявление"),
            new KeyboardButton("Поиск объявлений"),
            new KeyboardButton("Отменить действие")
        };

        var replyMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true
        };

        await botClient.SendTextMessageAsync(
            chatId,
            "Вы зарегистрированы! Выберите действие:",
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken
        );
    }


    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}