namespace Otus_Homework_8;

internal static class CustomLogger
{
    public static void Print(string message, string? text = null)
    {
        if (text != null) message = string.Format(message, text);

        Console.WriteLine(message);
    }
}