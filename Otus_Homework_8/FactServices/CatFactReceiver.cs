using System.Net.Http.Json;
using Otus_Homework_8.Exceptions;

namespace Otus_Homework_8.FactServices;

public class CatFactReceiver : IFactReceiver
{
    private const string CatFactUrl = "https://catfact.ninja/fact";
    private readonly HttpClient _client;

    public CatFactReceiver()
    {
        _client = new HttpClient();
    }

    public async Task<string> GetFactAsync(CancellationToken cancellationToken)
    {
        var fact = await _client.GetFromJsonAsync<CatFactDto>(CatFactUrl, cancellationToken);

        if (fact != null) return fact.Fact;

        throw new FactRetrievalException("Failed to retrieve cat fact.");
    }
}