using System.Net.Http.Json;
using SearchService.Models;

namespace SearchService.Services;

public class MovieServiceClient
{
    private readonly HttpClient _httpClient;

    public MovieServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Movie>> GetMoviesAsync(CancellationToken ct = default)
    {
        return await _httpClient.GetFromJsonAsync<List<Movie>>(
            "/movies",
            ct
        ) ?? new List<Movie>();
    }
}