namespace Otus_Homework_8.FactServices;

public interface IFactReceiver
{
    Task<string> GetFactAsync(CancellationToken cancellationToken);
}