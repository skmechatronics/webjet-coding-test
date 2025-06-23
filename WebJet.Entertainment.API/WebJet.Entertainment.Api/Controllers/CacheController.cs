//using Microsoft.AspNetCore.Mvc;
//using WebJet.Entertainment.Services;
//using WebJet.Entertainment.Services.Interfaces;

//namespace WebJet.Entertainment.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class CacheController : ControllerBase
//    {
//        private readonly IMoviesCacheService _cacheService;
//        private readonly IEnumerable<IMoviesHttpClient> _clients;

//        public CacheController(IMoviesCacheService cacheService, IEnumerable<IMoviesHttpClient> clients)
//        {
//            _cacheService = cacheService;
//            _clients = clients;
//        }

//        [HttpPost("update-cache")]
//        public async Task<IActionResult> RefreshCache()
//        {
//            await _cacheService.RefreshCacheAsync(_clients);
//            return Ok(new { Message = "Cache refresh triggered." });
//        }
//    }
//}