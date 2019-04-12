using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Models;

namespace WebJetMoviesAPI.Data.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(string endpoint, Lazy<HttpClient> htClient, ILogger<ApiService> logger, IMemoryCache memoryCache) 
            : base(endpoint, htClient, logger, memoryCache)
        {
        }
    }
}