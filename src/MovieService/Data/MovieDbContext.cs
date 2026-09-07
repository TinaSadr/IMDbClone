using Microsoft.EntityFrameworkCore;
using MovieService.Models;

namespace MovieService.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
}