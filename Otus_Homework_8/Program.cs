using DotNetEnv;
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
    private const string CancelMessage = "Requests was canceled.";

    private static async Task Main()
    {
        var cts = new CancellationTokenSource();

        Env.Load();

        var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            CustomLogger.Print(TokenNotFoundMessage);
            Environment.Exit(1);
        }

        var botClient = new TelegramBotClient(token);
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message],
            DropPendingUpdates = true
        };

        var handler = new UpdateHandler();

        handler.OnHandleUpdateStarted += HandleUpdateStarted;
        handler.OnHandleUpdateCompleted += HandleUpdateCompleted;

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