using MovieService.Data;
using MovieService.Models;
using MovieService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

builder.Services.AddHttpClient<TmdbService>(client =>
{
    client.BaseAddress = new Uri(
        "https://api.themoviedb.org/3/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// ============================================================
// GET ALL MOVIES
// ============================================================

app.MapGet("/movies", async (MovieDbContext db) =>
{
    return await db.Movies.ToListAsync();
});


// ============================================================
// GET MOVIE BY ID
// ============================================================

app.MapGet("/movies/{id}", async (int id, MovieDbContext db) =>
{
    var movie = await db.Movies.FindAsync(id);

    return movie is not null
        ? Results.Ok(movie)
        : Results.NotFound();
});


// ============================================================
// TEST TMDB POSTER API
// ============================================================

app.MapGet("/movies/{id}/poster-test", async (
    int id,
    MovieDbContext db,
    TmdbService tmdbService) =>
{
    var movie = await db.Movies.FindAsync(id);

    if (movie is null)
    {
        return Results.NotFound();
    }

    var posterPath =
        await tmdbService.GetPosterPathAsync(movie.TmdbId);

    return Results.Ok(new
    {
        movie.Id,
        movie.Title,
        movie.TmdbId,
        posterPath,

        posterUrl = posterPath is not null
            ? $"https://image.tmdb.org/t/p/w500{posterPath}"
            : null
    });
});


// ============================================================
// ENRICH ALL MOVIES WITH TMDB POSTERS
// ============================================================

app.MapPost("/movies/enrich-posters", async (
    MovieDbContext db,
    TmdbService tmdbService) =>
{
    var movies = await db.Movies
        .Where(m => string.IsNullOrWhiteSpace(m.PosterPath))
        .ToListAsync();

    var updated = 0;
    var processed = 0;

    foreach (var movie in movies)
    {
        processed++;

        try
        {
            var posterPath =
                await tmdbService.GetPosterPathAsync(movie.TmdbId);

            if (!string.IsNullOrWhiteSpace(posterPath))
            {
                movie.PosterPath = posterPath;
                updated++;
            }

            Console.WriteLine(
                $"[{processed}/{movies.Count}] " +
                $"{movie.Title} | Poster: {posterPath ?? "Not found"}");

            // Save every 25 movies so progress is not lost
            if (processed % 25 == 0)
            {
                await db.SaveChangesAsync();

                Console.WriteLine(
                    $"Saved progress. Posters added so far: {updated}");
            }

            // Small delay between TMDB requests
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"ERROR: {movie.Title} | {ex.Message}");
        }
    }

    // Save anything remaining
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        message = "Poster enrichment completed.",
        processed,
        updated
    });
});


app.Run();