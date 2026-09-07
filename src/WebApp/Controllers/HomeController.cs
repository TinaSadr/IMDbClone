using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("MovieService");

        try
        {
            var response = await client.GetAsync("/movies");

            if (!response.IsSuccessStatusCode)
                return View(new List<Movie>());

            var json = await response.Content.ReadAsStringAsync();

            var movies = JsonSerializer.Deserialize<List<Movie>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View(movies ?? new List<Movie>());
        }
        catch
        {
            return View(new List<Movie>());
        }
    }

   public async Task<IActionResult> Details(int id)
{
    var client = _httpClientFactory.CreateClient("MovieService");

    var response = await client.GetAsync($"/movies/{id}");

    if (!response.IsSuccessStatusCode)
    {
        return Content($"MovieService returned: {(int)response.StatusCode}");
    }

    var json = await response.Content.ReadAsStringAsync();

    var movie = JsonSerializer.Deserialize<Movie>(
        json,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    if (movie is null)
    {
        return Content("Movie could not be deserialized.");
    }

    return View(movie);
}
}