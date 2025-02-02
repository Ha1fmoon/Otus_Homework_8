using Otus_Homework_8.FactServices;
using Telegram.Bot.Types;

namespace Otus_Homework_8.MessageHandlers;

public class IncomingMessageHandler
{
    private readonly CommandHandler _commandHandler;
    private const char CommandTrigger = '/';

    public IncomingMessageHandler(IFactReceiver factReceiver)
    {
        _commandHandler = new CommandHandler(factReceiver);
    }

    public async Task<string> Handle(Update update, string response, CancellationToken cancellationToken)
    {
        if (update.Message?.Text is null || !update.Message.Text.StartsWith(CommandTrigger))
            return string.Format(response, update.Message?.Text);

        var command = update.Message.Text.Split(' ')[0].Remove(0, 1);

        return await _commandHandler.Handle(command, cancellationToken);
    }
}