using System;
using System.Net.Http;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Data.Repository;

namespace WebJetMoviesAPI.Data
{
    public class ApiService : IApiService
    {
        
        public ApiService(HttpClient httpClient)
        {
            var lazyHttpClient = new Lazy<HttpClient>(() => httpClient);
            Cinemaworld = new MovieRepository("cinemaworld", lazyHttpClient);
            Filmworld = new MovieRepository("filmworld", lazyHttpClient);
        }
        
        public IMovieRepository Cinemaworld { get; }
        public IMovieRepository Filmworld { get; }
    }
}