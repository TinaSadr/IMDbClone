using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlite("Data Source=movies.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/movies", async (MovieDbContext db) =>
{
    return await db.Movies.ToListAsync();
});

app.MapGet("/movies/{id}", async (int id, MovieDbContext db) =>
{
    var movie = await db.Movies.FindAsync(id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});
    

app.Run();