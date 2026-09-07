namespace SearchService.Models;

public class MovieEmbedding
{
    public int MovieId { get; set; }

    public string Title { get; set; } = string.Empty;

    public float[] Embedding { get; set; } = Array.Empty<float>();
}