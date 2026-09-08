
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<SemanticSearchService>();

builder.Services.AddHttpClient<MovieServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MovieService:BaseUrl"] ?? throw new InvalidOperationException("MovieService base URL is missing."));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Health check
app.MapGet("/", () =>
{
    return "SearchService is running.";
});

// Semantic search
app.MapGet("/search", async (
    string q,
    SemanticSearchService searchService,
    MovieServiceClient movieService,
    CancellationToken ct) => 
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new
        {
            error = "Search query is required."
        });
    }

    var results = await searchService.SearchAsync(q, ct);

    var movies = await movieService.GetMoviesAsync(ct);

    var response = results
        .Join(
            movies,
            result => result.MovieId,
            movie => movie.Id,
            (result, movie) => new
            {
                movie.Id,
                movie.Title,
                movie.Overview,
                movie.Genres,
                Similarity = Math.Round(result.Score, 4)
            })
        .ToList();

    return Results.Ok(response);
});

// Create movie embedding index
app.MapPost("/index", async (
    MovieServiceClient movieService,
    EmbeddingService embeddingService,
    CancellationToken ct) =>
{
    var movies = await movieService.GetMoviesAsync(ct);

    if (movies.Count == 0)
    {
        return Results.BadRequest(new
        {
            error = "No movies were returned from MovieService."
        });
    }

    var embeddings = new List<SearchService.Models.MovieEmbedding>();

    for (int i = 0; i < movies.Count; i++)
    {
        // This stops the loop immediately if you cancel the process or stop debugging
        ct.ThrowIfCancellationRequested();

        var movie = movies[i];

        var text = $"""
            Title: {movie.Title}
            Original Title: {movie.OriginalTitle}
            Overview: {movie.Overview}
            Tagline: {movie.Tagline}
            Genres: {movie.Genres}
            Keywords: {movie.Keywords}
            """;

        Console.WriteLine(
            $"Embedding movie {i + 1} of {movies.Count}: {movie.Title}"
        );

        var embedding =
            await embeddingService.GenerateEmbeddingAsync(text, ct);

        embeddings.Add(
            new SearchService.Models.MovieEmbedding
            {
                MovieId = movie.Id,
                Title = movie.Title,
                Embedding = embedding
            });
    }

    var path = Path.Combine(
        app.Environment.ContentRootPath,
        "movie-embeddings.json"
    );

    var json = System.Text.Json.JsonSerializer.Serialize(embeddings);

    // Pass the token to the file writer
    await File.WriteAllTextAsync(path, json, ct);

    return Results.Ok(new
    {
        message = "Movie embeddings created successfully.",
        count = embeddings.Count
    });
});

app.Run();