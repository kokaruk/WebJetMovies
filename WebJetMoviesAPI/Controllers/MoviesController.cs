using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Models;
using WebJetMoviesAPI.Utils;
using WebJetMoviesAPI.Utils.SettingsModels;

namespace WebJetMoviesAPI.Controllers
{
    [EnableCors("MaxOpen")]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
    public class MoviesController : ControllerBase
    {
        private const string CollectionEndpoint = "movies";
        private const string SingleEndpoint = "movie";

        private readonly IApiService _apiService;
        private readonly IOptions<PaginationOptions> _paginationSettings;
        private readonly IPosterService _posterService;

        public MoviesController(IApiService apiService,
            IPosterService posterService,
            IOptions<PaginationOptions> paginationSettings)
        {
            _apiService = apiService;
            _posterService = posterService;
            _paginationSettings = paginationSettings;
        }

        /// <summary>
        ///     Gets full list of movies or paginates if provided page number
        /// </summary>
        [HttpGet]
        [HttpGet("{page?}")]
        public async Task<ActionResult<PageCollectionResponse<TupleWrapperResponse<Movie>>>> GetAll(int? page)
        {
            var allRequests = _apiService.MovieServices.Values.ToList().Select(i => i.GetAllAsync(CollectionEndpoint));
            var result = await Task.WhenAll(allRequests);
            var distinctMovies = result.SelectMany(c => c).GroupBy(m => m.Title)
                .Select(g => g.First())
                .OrderBy(m => m.Year)
                .ToList();

            if (!distinctMovies.Any())
                return NoContent();

            if (page == null || page.Value == 0)
                return Ok(new PageCollectionResponse<Movie> {Items = await FixPosterAddresses(distinctMovies)});

            // return paginated data
            if (!distinctMovies
                .Skip((page.Value - 1) * _paginationSettings.Value.ItemsLimit)
                .Take(_paginationSettings.Value.ItemsLimit).Any())
                return NoContent();

            var pageListingMovies = distinctMovies
                .Skip((page.Value - 1) * _paginationSettings.Value.ItemsLimit)
                .Take(_paginationSettings.Value.ItemsLimit)
                .OrderBy(m => m.Year)
                .ToList();

            var baseUrl = Regex.Replace(Url.Action("GetAll"), @"\d*$", string.Empty);

            var nextHasData = distinctMovies
                .Skip(page.Value * _paginationSettings.Value.ItemsLimit)
                .Take(_paginationSettings.Value.ItemsLimit)
                .Any();

            var prevHasData = page.Value - 1 > 0 && distinctMovies
                                  .Skip((page.Value - 2) * _paginationSettings.Value.ItemsLimit)
                                  .Take(_paginationSettings.Value.ItemsLimit)
                                  .Any();

            // leave this here if decide to have pull prices on initial display
            /*
            var maxItems = new List<CheapestMovieResponse<Movie>>();
            pageListingMovies.ForEach( mv =>
            {
                var cheapestResponse = GetCheapestMovie(mv.Year, mv.Title).Result;
                maxItems.Add(cheapestResponse);
            });
            */
            return Ok(new PageCollectionResponse<Movie>
            {
//                    Items = pageListingMovies, <- this is also part of commented code above
                Items = await FixPosterAddresses(pageListingMovies),
                NextPage = nextHasData ? new Uri($"{baseUrl}{page.Value + 1}") : null,
                PreviousPage = prevHasData ? new Uri($"{baseUrl}{page.Value - 1}") : null
            });
        }

        /// <summary>
        ///     get single movie based on title and year
        /// </summary>
        [HttpGet("{year}/{title}")]
        public async Task<ActionResult<TupleWrapperResponse<Movie>>> Get(string year, string title)
        {
            if (string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(title))
                BadRequest();

            var cheapestMovie = await GetCheapestMovie(year, title);

            return cheapestMovie != null ? Ok(cheapestMovie) : (ActionResult) NoContent();
        }


        private async Task<TupleWrapperResponse<Movie>> GetCheapestMovie(string year, string title)
        {
            var moviesCollections = new Dictionary<string, Task<Movie>>();

            _apiService.MovieServices.Keys.ToList().ForEach(key =>
                moviesCollections.Add(key,
                    _apiService.MovieServices[key]
                        .FindAsync(CollectionEndpoint, m => m.Year.Equals(year) && m.Title.Equals(title))));

            await Task.WhenAll(moviesCollections.Values);

            var requiredMovies = await UpdateCollections(moviesCollections
                .Where(f => f.Value.Result != null)
                .ToDictionary(x => x.Key, x => x.Value));

            if (!requiredMovies.Any())
                return null;

            var (parent, movie) = requiredMovies
                .FirstOrDefault(x => x.Value.Price.Equals(requiredMovies.Values.Min(m => m.Price)));
            movie.Poster = await FixPosterAddress(movie);

            return new TupleWrapperResponse<Movie> {ParentName = parent, Member = movie};
        }

        /// <summary>
        ///     Updates dict of movies received from list request endpoint to individual request endpoint
        /// </summary>
        private async Task<Dictionary<string, Movie>> UpdateCollections(
            Dictionary<string, Task<Movie>> moviesCollections)
        {
            moviesCollections.Keys.ToList().ForEach(key =>
                moviesCollections[key] = _apiService.MovieServices[key]
                    .GetAsync(SingleEndpoint, moviesCollections[key].Result.Id)
            );
            await Task.WhenAll(moviesCollections.Values);
            return moviesCollections.ToDictionary(x => x.Key, x => x.Value.Result);
        }

        #region update posters for movies

        /// <summary>
        ///     Update movies poster url from movies database
        /// </summary>
        private async Task<IEnumerable<Movie>> FixPosterAddresses(List<Movie> movies)
        {
            foreach (var movie in movies) movie.Poster = await FixPosterAddress(movie);

            return movies;
        }

        private async Task<string> FixPosterAddress(Movie movie)
        {
            return await _posterService.GetAsync(movie.Title, movie.Year);
        }

        #endregion
    }
}
