namespace Otus_Homework_8.Exceptions;

public class FactRetrievalException : Exception
{
    public FactRetrievalException(string message, Exception? innerException = null)
        : base(message, innerException){}
}