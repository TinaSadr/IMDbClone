using System.Text.Json;
using SearchService.Models;

namespace SearchService.Services;

public class SemanticSearchService
{
    private readonly EmbeddingService _embeddingService;
    private readonly string _indexPath;

    public SemanticSearchService(
        EmbeddingService embeddingService,
        IWebHostEnvironment environment)
    {
        _embeddingService = embeddingService;

        _indexPath = Path.Combine(
            environment.ContentRootPath,
            "movie-embeddings.json"
        );
    }

    public async Task<List<MovieEmbedding>> LoadEmbeddingsAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_indexPath))
        {
            return new List<MovieEmbedding>();
        }

        var json = await File.ReadAllTextAsync(_indexPath, ct);

        return JsonSerializer.Deserialize<List<MovieEmbedding>>(json)
            ?? new List<MovieEmbedding>();
    }

    public async Task<List<(int MovieId, double Score)>> SearchAsync(
        string query,
        CancellationToken ct = default,
        int limit = 10)
    {
        var movies = await LoadEmbeddingsAsync(ct);

        if (movies.Count == 0)
        {
            return new List<(int, double)>();
        }

        var queryEmbedding =
            await _embeddingService.GenerateEmbeddingAsync(query, ct);

        var results = movies
            .Select(movie => (
                MovieId: movie.MovieId,
                Score: CosineSimilarity(
                    queryEmbedding,
                    movie.Embedding)
            ))
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .ToList();

        return results;
    }

    private static double CosineSimilarity(
        float[] a,
        float[] b)
    {
        if (a.Length != b.Length)
        {
            return 0;
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];

            magnitudeA += a[i] * a[i];

            magnitudeB += b[i] * b[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct /
               (Math.Sqrt(magnitudeA) *
                Math.Sqrt(magnitudeB));
    }
}