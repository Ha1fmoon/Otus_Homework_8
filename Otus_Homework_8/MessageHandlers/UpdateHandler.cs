using Otus_Homework_8.FactServices;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Otus_Homework_8.MessageHandlers;

public class UpdateHandler : IUpdateHandler
{
    private readonly string _response;
    private readonly IncomingMessageHandler _messageHandler;

    public delegate Task MessageHandler(Update update);

    public event MessageHandler? OnHandleUpdateStarted;
    public event MessageHandler? OnHandleUpdateCompleted;
    public event Action<Exception>? OnErrorOccurred;

    public UpdateHandler(string response, IFactReceiver factReceiver)
    {
        _response = response;
        _messageHandler = new IncomingMessageHandler(factReceiver);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is null && update.Message?.From is null) return;

        if (OnHandleUpdateStarted != null) await OnHandleUpdateStarted.Invoke(update);

        try
        {
            var response = await _messageHandler.Handle(update, _response, cancellationToken);

            await botClient.SendMessage(update.Message.Chat.Id, response, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            OnErrorOccurred?.Invoke(exception);
        }
        finally
        {
            if (OnHandleUpdateCompleted != null) await OnHandleUpdateCompleted.Invoke(update);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        if (OnErrorOccurred != null) OnErrorOccurred?.Invoke(exception);

        return Task.CompletedTask;
    }
}