using CsvHelper;
using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Models;

var projectRoot = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")
);

var csvPath = Path.Combine(
    projectRoot,
    "data",
    "tmdb",
    "tmdb_5000_movies.csv"
);

var databasePath = Path.Combine(
    projectRoot,
    "src",
    "MovieService",
    "movies.db"
);

Console.WriteLine("TMDB Movie Importer");
Console.WriteLine("-------------------");

if (!File.Exists(csvPath))
{
    Console.WriteLine($"CSV file not found: {csvPath}");
    return;
}

if (!File.Exists(databasePath))
{
    Console.WriteLine($"Database not found: {databasePath}");
    return;
}

var options = new DbContextOptionsBuilder<MovieDbContext>()
    .UseSqlite($"Data Source={databasePath}")
    .Options;

await using var db = new MovieDbContext(options);

using var reader = new StreamReader(csvPath);
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

var records = csv.GetRecords<TmdbMovieRecord>().ToList();

Console.WriteLine($"Found {records.Count} movies.");

foreach (var record in records)
{
    var movie = new Movie
    {
        TmdbId = record.id,
        Title = record.title ?? string.Empty,
        OriginalTitle = record.original_title ?? string.Empty,
        Overview = record.overview ?? string.Empty,
        Tagline = record.tagline ?? string.Empty,
        Genres = ExtractNames(record.genres),
        Keywords = ExtractNames(record.keywords),
        OriginalLanguage = record.original_language ?? string.Empty,
        ReleaseDate = ParseDate(record.release_date),
        Runtime = ParseInt(record.runtime),
        Popularity = ParseDouble(record.popularity),
        VoteAverage = ParseDouble(record.vote_average),
        VoteCount = ParseInt(record.vote_count) ?? 0,
        Budget = ParseLong(record.budget),
        Revenue = ParseLong(record.revenue),
        Status = record.status ?? string.Empty
    };

    db.Movies.Add(movie);
}

await db.SaveChangesAsync();

Console.WriteLine($"Imported {records.Count} movies successfully!");

static string ExtractNames(string? json)
{
    if (string.IsNullOrWhiteSpace(json))
        return string.Empty;

    try
    {
        using var document = JsonDocument.Parse(json);

        return string.Join(
            ", ",
            document.RootElement
                .EnumerateArray()
                .Select(x => x.GetProperty("name").GetString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
        );
    }
    catch
    {
        return string.Empty;
    }
}

static DateTime? ParseDate(string? value)
{
    if (DateTime.TryParse(
        value,
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out var date))
    {
        return date;
    }

    return null;
}

static int? ParseInt(string? value)
{
    return int.TryParse(
        value,
        NumberStyles.Integer,
        CultureInfo.InvariantCulture,
        out var result)
        ? result
        : null;
}

static long ParseLong(string? value)
{
    return long.TryParse(
        value,
        NumberStyles.Integer,
        CultureInfo.InvariantCulture,
        out var result)
        ? result
        : 0;
}

static double ParseDouble(string? value)
{
    return double.TryParse(
        value,
        NumberStyles.Float,
        CultureInfo.InvariantCulture,
        out var result)
        ? result
        : 0;
}

public class TmdbMovieRecord
{
    public int id { get; set; }
    public string? title { get; set; }
    public string? original_title { get; set; }
    public string? overview { get; set; }
    public string? tagline { get; set; }
    public string? genres { get; set; }
    public string? keywords { get; set; }
    public string? original_language { get; set; }
    public string? release_date { get; set; }
    public string? runtime { get; set; }
    public string? popularity { get; set; }
    public string? vote_average { get; set; }
    public string? vote_count { get; set; }
    public string? budget { get; set; }
    public string? revenue { get; set; }
    public string? status { get; set; }
}
