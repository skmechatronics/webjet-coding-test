using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using WebJet.Entertainment.Services.ApiModels;
using WebJet.Entertainment.Services.Interfaces;
using WebJet.Entertainment.Services.UiModels;
using Xunit;

namespace WebJet.Entertainment.Services.Tests;

public class MoviesAggregatorServiceTests
{
    private readonly string _title1 = "Inception";
    private readonly string _title2 = "Interstellar";

    private readonly MovieSource _source1 = new() { Name = "Source1" };
    private readonly MovieSource _source2 = new() { Name = "Source2" };

    private readonly List<Movie> _source1Movies;
    private readonly List<Movie> _source2Movies;

    private readonly MovieMetadata _metadata1;
    private readonly MovieMetadata _metadata2;

    private readonly FakeMoviesCacheService _cache;
    private readonly MoviesAggregatorService _service;
    private static readonly string[] expected = new[] { "Source1", "Source2" };

    public MoviesAggregatorServiceTests()
    {
        _source1Movies = new()
        {
            new Movie { Id = "s1-1", Title = _title1 },
            new Movie { Id = "s1-2", Title = _title2 }
        };

        _source2Movies = new()
        {
            new Movie { Id = "s2-1", Title = _title1 },
            new Movie { Id = "s2-2", Title = _title2 }
        };

        _metadata1 = new MovieMetadata { Title = _title1, Price = 10m };
        _metadata2 = new MovieMetadata { Title = _title1, Price = 25m };

        _cache = new FakeMoviesCacheService();

        // Pre-cache movie collections and individual movies for both sources
        _cache.CacheMoviesCollection(_source1, new MoviesCollection { Movies = _source1Movies }).Wait();
        _cache.CacheMoviesCollection(_source2, new MoviesCollection { Movies = _source2Movies }).Wait();

        var logger = new Mock<ILogger<MoviesAggregatorService>>();

        var client1 = new Mock<IMoviesHttpClient>();
        client1.SetupGet(c => c.Source).Returns(_source1);
        client1.Setup(c => c.GetMovieById("s1-1"))
            .ReturnsAsync(new SuccessResult<MovieMetadata> { Data = _metadata1 });

        var client2 = new Mock<IMoviesHttpClient>();
        client2.SetupGet(c => c.Source).Returns(_source2);
        client2.Setup(c => c.GetMovieById("s2-1"))
            .ReturnsAsync(new SuccessResult<MovieMetadata> { Data = _metadata2 });

        _service = new MoviesAggregatorService(logger.Object, new[] { client1.Object, client2.Object }, _cache);
    }

    [Fact]
    public async Task GetMovieMetadataByPriceAsync_ShouldReturnSortedMetadataAndCacheIt_FromMultipleSourcesWithMultipleMovies()
    {
        // Act
        var result = await _service.GetMovieMetadataByPriceAsync(_title1);

        // Assert
        result.ShouldBeOfType<SuccessResult<MovieMetadataModel>>();
        var model = (result as SuccessResult<MovieMetadataModel>)!.Data;

        model.Title.ShouldBe(_title1);
        model.SourcePrices.Count.ShouldBe(2);
        model.SourcePrices[0].Price.ShouldBe(10m);
        model.SourcePrices[1].Price.ShouldBe(25m);

        model.SourcePrices[0].Source.ShouldBe("Source1");
        model.SourcePrices[1].Source.ShouldBe("Source2");

        _cache.MetadataCache.ContainsKey(("Source1", _title1)).ShouldBeTrue();
        _cache.MetadataCache.ContainsKey(("Source2", _title1)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllMovies_ShouldUseCachedCollections_AndReturnMergedMoviesFromBothSources()
    {
        // Act
        var result = await _service.GetAllMovies();

        // Assert
        result.ShouldNotBeNull();
        result.Movies.Count.ShouldBe(2); // Inception and Interstellar

        var inception = result.Movies.Single(m => m.Title == _title1);
        var interstellar = result.Movies.Single(m => m.Title == _title2);

        inception.AvailableSources.ShouldBeSubsetOf(["Source1", "Source2"]);
        interstellar.AvailableSources.ShouldBeSubsetOf(expected);

        _cache.MovieCache.ContainsKey(("Source1", _title1)).ShouldBeTrue();
        _cache.MovieCache.ContainsKey(("Source2", _title1)).ShouldBeTrue();
        _cache.MovieCache.ContainsKey(("Source1", _title2)).ShouldBeTrue();
        _cache.MovieCache.ContainsKey(("Source2", _title2)).ShouldBeTrue();
    }
}
