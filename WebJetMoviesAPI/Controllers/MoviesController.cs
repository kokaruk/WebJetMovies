using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Models;
using WebJetMoviesAPI.Utils;

namespace WebJetMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
    public class MoviesController : ControllerBase
    {
        private const string CollectionEndpoint = "movies";
        private const string _singleEndpoint = "movie";

        private readonly IApiService _apiService;

        public MoviesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> Get()
        {
            var allRequests = _apiService.MovieServices.Values.ToList().Select(i => i.GetAllAsync(CollectionEndpoint));
            var result = await Task.WhenAll(allRequests);
            var distinctMovies = result.SelectMany(c => c).GroupBy(m => m.Title)
                .Select(g => g.First())
                .OrderBy(m => m.Year)
                .ToList();

            return Ok(distinctMovies);
        }

        [HttpGet("{year}/{title}")]
        public async Task<ActionResult<Movie>> Get(string year, string title)
        {
            
            var moviesCollections = new Dictionary<string, Task<Movie>>();
            // avoiding access to modified collection
            var collections = moviesCollections;
            _apiService.MovieServices.Keys.ToList().ForEach(   key => 
                collections.Add(key, 
                      _apiService.MovieServices[key]
                        .FindAsync(CollectionEndpoint, m => m.Year.Equals(year) && m.Title.Equals(title))));
            
            await Task.WhenAll(moviesCollections.Values);
            
            moviesCollections = await UpdateCollections(moviesCollections);

            var mpv = moviesCollections[moviesCollections.Keys.First()].Result;
            
            return Ok(mpv);
        }

        private async Task<Dictionary<string, Task<Movie>>> UpdateCollections(Dictionary<string, Task<Movie>> moviesCollections)
        {
            moviesCollections.Keys.ToList().ForEach(key => moviesCollections[key]=
                _apiService.MovieServices[key].GetAsync(_singleEndpoint, moviesCollections[key].Result.Id)
                );
            await Task.WhenAll(moviesCollections.Values);
            return moviesCollections;
        }
        
    }
}