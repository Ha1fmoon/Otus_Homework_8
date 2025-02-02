using Otus_Homework_8.FactServices;

namespace Otus_Homework_8.MessageHandlers;

public class CommandHandler
{
    private readonly IFactReceiver _factReceiver;

    public CommandHandler(IFactReceiver factReceiver)
    {
        _factReceiver = factReceiver;
    }

    public async Task<string> Handle(string command, CancellationToken cancellationToken)
    {
        switch (command)
        {
            case "cat":
                return await _factReceiver.GetFactAsync(cancellationToken);
            default:
                throw new NotImplementedException($"Command not implemented: {command}");
        }
    }
}