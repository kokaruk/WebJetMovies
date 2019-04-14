using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Models;

namespace WebJetMoviesAPI.Data.Repository
{
    /// <summary>
    ///     concrete implementation of repository
    /// </summary>
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(string endpoint, Lazy<HttpClient> htClient, ILogger<ApiService> logger,
            IMemoryCache memoryCache)
            : base(endpoint, htClient, logger, memoryCache)
        {
        }
    }
}