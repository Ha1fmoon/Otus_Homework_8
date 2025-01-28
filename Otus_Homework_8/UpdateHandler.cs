using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Otus_Homework_8;

public class UpdateHandler : IUpdateHandler
{
    private const string CatFactCommand = "/cat";
    private const string Response = "Message {0} was successfully handled.";
    private const string CatFactUrl = "https://catfact.ninja/fact";
    private const string WarningMessage = "Something went wrong";
    private const string CatFactCanceledMessage = "Request to catfact was canceled.";
    private const string ErrorLogMessage = "Error occurred: {0}";

    public delegate Task MessageHandler(Update update);

    public event MessageHandler? OnHandleUpdateStarted;
    public event MessageHandler? OnHandleUpdateCompleted;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message?.From is null) return;

        if (OnHandleUpdateStarted != null) await OnHandleUpdateStarted.Invoke(update);

        string response;

        if (update.Message.Text == CatFactCommand)
        {
            response = await GetCatFact(cancellationToken);
        }
        else
        {
            response = string.Format(Response, update.Message.Text);
        }

        await botClient.SendMessage(update.Message.Chat.Id, response, cancellationToken: cancellationToken);

        if (OnHandleUpdateCompleted != null) await OnHandleUpdateCompleted.Invoke(update);
    }

    private async Task<string> GetCatFact(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        try
        {
            var fact = await client.GetFromJsonAsync<CatFactDto>(CatFactUrl, cancellationToken);
            return fact?.Fact ?? WarningMessage;
        }
        catch (OperationCanceledException)
        {
            return CatFactCanceledMessage;
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        CustomLogger.Print(ErrorLogMessage, exception.Message);
        return Task.CompletedTask;
    }
}