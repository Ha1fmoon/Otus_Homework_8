using DotNetEnv;
using Otus_Homework_8.FactServices;
using Otus_Homework_8.MessageHandlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Otus_Homework_8;

internal class Program
{
    private const string StartHandleMessage = "Message handling start '{0}'";
    private const string EndHandleMessage = "Message handling ended '{0}'";
    private const string BotInfoMessage = "Bot info: {0}";
    private const string BotStartMessage = "{0} started.";
    private const string QuitMessage = "Quitting...";
    private const string PressToQuitMessage = "Press 'A' to quit.";
    private const string TokenNotFoundMessage = "Token not found.";
    private const string Response = "Message {0} was successfully handled.";
    private const string ErrorOccurredMessage = "Error occurred. Exception: {0}";

    private static async Task Main()
    {
        var cts = new CancellationTokenSource();

        Env.Load();

        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            throw new Exception(TokenNotFoundMessage);
        }

        var botClient = new TelegramBotClient(token);
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message],
            DropPendingUpdates = true
        };
        
        var factService = new CatFactReceiver();

        var handler = new UpdateHandler(Response, factService);

        handler.OnHandleUpdateStarted += HandleUpdateStarted;
        handler.OnHandleUpdateCompleted += HandleUpdateCompleted;
        handler.OnErrorOccurred += HandleError;

        try
        {
            botClient.StartReceiving(handler, receiverOptions, cts.Token);

            var me = await botClient.GetMe(cts.Token);
            CustomLogger.Print(BotStartMessage, me.FirstName);

            await Task.Run(() => HandleInput(botClient, cts), cts.Token);
        }
        finally
        {
            handler.OnHandleUpdateStarted -= HandleUpdateStarted;
            handler.OnHandleUpdateCompleted -= HandleUpdateCompleted;
            handler.OnErrorOccurred -= HandleError;
        }
    }

    private static Task HandleUpdateStarted(Update update)
    {
        CustomLogger.Print(string.Format(StartHandleMessage, update.Message?.Text));
        return Task.CompletedTask;
    }

    private static Task HandleUpdateCompleted(Update update)
    {
        CustomLogger.Print(string.Format(EndHandleMessage, update.Message?.Text));
        return Task.CompletedTask;
    }
    
    private static void HandleError(Exception exception)
    {
        CustomLogger.Print(string.Format(ErrorOccurredMessage, exception.Message));
    }


    private static async Task HandleInput(TelegramBotClient botClient, CancellationTokenSource cts)
    {
        CustomLogger.Print(PressToQuitMessage);
        while (!cts.Token.IsCancellationRequested)
        {
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.A)
            {
                CustomLogger.Print(QuitMessage);
                await cts.CancelAsync();
                break;
            }

            var me = await botClient.GetMe(cts.Token);
            CustomLogger.Print(string.Format(BotInfoMessage, me.Username));
        }
    }
}