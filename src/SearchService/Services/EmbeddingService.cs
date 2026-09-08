using OpenAI;
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
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        // Initialize the main client first, then generate the EmbeddingClient
        var openAiClient = new OpenAIClient(apiKey);
        _client = openAiClient.GetEmbeddingClient("text-embedding-3-small");
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        // Passing the cancellation token to the OpenAI SDK
        var result = await _client.GenerateEmbeddingAsync(text, cancellationToken: ct);

        return result.Value.ToFloats().ToArray();
    }
}