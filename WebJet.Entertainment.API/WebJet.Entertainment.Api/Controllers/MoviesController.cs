using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;
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
    [SwaggerOperation(
        Summary = "Get movies from all sources",
        Description = "Fetches the full list of available movies from all configured external providers.",
        OperationId = "GetAllMovies",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "List of available movies", typeof(IEnumerable<MovieModel>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "No movies were found")]
    public async Task<IActionResult> GetMovieCollection()
    {
        _logger.LogInformation("Getting movies");
        var movieCollection = await _moviesAggregatorService.GetAllMovies();
        if (movieCollection.Movies.Any())
        {
            return Ok(movieCollection.Movies);
        }

        return NotFound("No movies were found in our sources.");
    }

    [HttpGet("{title}/price-comparison")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = "Get cheapest movie metadata",
        Description = "Returns metadata from the cheapest provider for the given movie title.",
        OperationId = "GetCheapestMovieMetadata",
        Tags = new[] { "Movies" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful lookup", typeof(MovieMetadataModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Title was missing or invalid")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No movie found with that title")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Upstream services unavailable")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error")]
    public async Task<IActionResult> GetCheapestMovieMetadata(
        [FromRoute][SwaggerParameter("Movie title to search for", Required = true)]  string title)
    {
        title = title.Trim();
        if (string.IsNullOrEmpty(title))
        {
            return BadRequest($"Title must be provided but was empty");
        }

        var result = await _moviesAggregatorService.GetMovieMetadataByPriceAsync(title);
        return result switch
        {
            SuccessResult<MovieMetadataModel> success => Ok(success.Data),

            ErrorResult<MovieMetadataModel> error
            when error.ErrorCode is ErrorCodes.MovieNotFound or ErrorCodes.NoMoviesFound =>
                NotFound(new
                {
                    title = "Movie not found",
                    detail = error.ErrorMessage
                }),

            ErrorResult<MovieMetadataModel> error =>
                Problem(
                    detail: error.ErrorMessage,
                    statusCode: (int)HttpStatusCode.ServiceUnavailable,
                    title: "Movie not found"),

            _ => StatusCode(500, "Unexpected error")
        };
    }
}