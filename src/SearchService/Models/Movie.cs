namespace SearchService.Models;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string OriginalTitle { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public string Tagline { get; set; } = string.Empty;

    public string Genres { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;
}