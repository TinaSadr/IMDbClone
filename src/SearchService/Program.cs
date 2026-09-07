using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<SemanticSearchService>();

builder.Services.AddHttpClient<MovieServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5132");
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
    MovieServiceClient movieService) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new
        {
            error = "Search query is required."
        });
    }

    var results = await searchService.SearchAsync(q);

    var movies = await movieService.GetMoviesAsync();

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
    EmbeddingService embeddingService) =>
{
    var movies = await movieService.GetMoviesAsync();

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
            await embeddingService.GenerateEmbeddingAsync(text);

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

    await File.WriteAllTextAsync(path, json);

    return Results.Ok(new
    {
        message = "Movie embeddings created successfully.",
        count = embeddings.Count
    });
});

app.Run();