using System.Net.Http.Headers;
using System.Text.Json;

namespace MovieService.Services;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public TmdbService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _token = configuration["TMDB:ApiToken"]
            ?? throw new InvalidOperationException(
                "TMDB API token is missing.");
    }

    public async Task<string?> GetPosterPathAsync(int tmdbId)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"movie/{tmdbId}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        if (document.RootElement.TryGetProperty(
                "poster_path",
                out var posterPath))
        {
            return posterPath.GetString();
        }

        return null;
    }
}