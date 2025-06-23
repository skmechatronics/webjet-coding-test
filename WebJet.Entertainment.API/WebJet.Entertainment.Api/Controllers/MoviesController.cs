using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.UiModels;

namespace WebJet.Entertainment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesAggregatorService _moviesAggregatorService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(
        IMoviesAggregatorService moviesService,
        ILogger<MoviesController> logger)
    {
        _moviesAggregatorService = moviesService;
        _logger = logger;
    }

    [HttpGet("movie-collection")]
    public async Task<IActionResult> GetMovieCollection()
    {
        _logger.LogInformation("Getting movies");
        var movies = await _moviesAggregatorService.GetAllMovies();
        return Ok(movies);
    }

    [HttpGet("{title}/price-comparison")]
    public async Task<IActionResult> GetCheapestMovieMetadata([FromRoute] string title)
    {
        var result = await _moviesAggregatorService.GetMovieMetadataByPriceAsync(title);

        return result switch
        {
            SuccessResult<MovieMetadataModel> success => Ok(success.Data),
            ErrorResult<MovieMetadataModel> error => Problem(
                detail: error.ErrorMessage,
                statusCode: ((int)HttpStatusCode.ServiceUnavailable),
                title: "Movie not found"),
            _ => StatusCode(500, "Unexpected error")
        };
    }
}