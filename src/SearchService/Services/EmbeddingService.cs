using OpenAI.Embeddings;

namespace SearchService.Services;

public class EmbeddingService
{
    private readonly EmbeddingClient _client;

    public EmbeddingService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is missing."
            );
        }

        _client = new EmbeddingClient(
            "text-embedding-3-small",
            apiKey
        );
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var result = await _client.GenerateEmbeddingAsync(text);

        return result.Value.ToFloats().ToArray();
    }
}