using Microsoft.Extensions.Configuration;
using System.Text.Json;
using WebJet.Entertainment.Poc;

namespace WebJet.Entertainment.PoC;

internal class Program
{
    private const string ENVIRONMENT_KEY = "DOTNET_ENVIRONMENT";

    static async Task Main(string[] args)
    {
        var aspEnvironment = Environment.GetEnvironmentVariable(ENVIRONMENT_KEY) ?? "Development";
        Console.WriteLine($"Using Environment: {aspEnvironment}");

        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile($"appSettings.{aspEnvironment}.json", true)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();


        var webJetConfiguration = configuration.GetSection(WebJetConfiguration.SectionName).Get<WebJetConfiguration>();
        if (webJetConfiguration is null || webJetConfiguration.IsMisconfigured())
        {
            Console.WriteLine("Expected configuration values in README.md");
            return;
        }

        var cinemaworldMovies = await GetMovieList<MovieCollection>(
            webJetConfiguration.CinemaworldApiBaseUrl, webJetConfiguration.ApiKey);

        Console.WriteLine($"There were {cinemaworldMovies?.Movies.Count() ?? 0} movies from cinemaworld.");
        Console.WriteLine(string.Join(Environment.NewLine, cinemaworldMovies.Movies.Select(i => i.Title)));

        var cinemaworldDetails = await GetMovie<MovieDetails>(
            webJetConfiguration.CinemaworldApiBaseUrl, webJetConfiguration.ApiKey, "cw0076759");

        Console.WriteLine($"");

        //var filmworldMovies = await GetMovieList<MovieCollection>(webJetConfiguration.FilmworldApiBaseUrl, webJetConfiguration.ApiKey);
        //Console.WriteLine($"There were {filmworldMovies?.Movies.Count() ?? 0} movies from cinemaworld.");
        //Console.WriteLine(string.Join(Environment.NewLine, filmworldMovies.Movies.Select(i => i.Title)));

        //var filmworldDetails = await GetMovie<MovieDetails>(
        //    webJetConfiguration.FilmworldApiBaseUrl, webJetConfiguration.ApiKey, "cw0076759");
    }

    private static async Task<T?> GetMovieList<T>(string url, string apiKey)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(url.EnsureTrailingSlash())
        };
        client.DefaultRequestHeaders.Add("x-access-token", apiKey);

        var result = await client.GetAsync("movies");
        if (result.IsSuccessStatusCode)
        {
            var rawContent = await result.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<T>(rawContent);
            return content;
        }
        else
        {
            Console.WriteLine("Failed to fetch...");
            return default;
        }
    }

    private static async Task<T?> GetMovie<T>(string url, string apiKey, string id)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(url.EnsureTrailingSlash())
        };

        client.DefaultRequestHeaders.Add("x-access-token", apiKey);
        var result = await client.GetAsync($"movie/{id}");
        if (result.IsSuccessStatusCode)
        {
            var rawContent = await result.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<T>(rawContent);
            return content;
        }
        else
        {
            Console.WriteLine("Failed to fetch...");
            return default;
        }

    }
}
