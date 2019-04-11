using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Data.Repository;

namespace WebJetMoviesAPI.Data
{
    public class ApiService : IApiService
    {
        
        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            var lazyHttpClient = new Lazy<HttpClient>(() => httpClient);
            Cinemaworld = new MovieRepository("cinemaworld", lazyHttpClient,  logger);
            Filmworld = new MovieRepository("filmworld", lazyHttpClient, logger);
        }
        
        public IMovieRepository Cinemaworld { get; }
        public IMovieRepository Filmworld { get; }
    }
}