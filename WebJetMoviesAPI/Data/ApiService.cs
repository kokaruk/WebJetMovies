using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Data.Repository;
using WebJetMoviesAPI.Utils;

namespace WebJetMoviesAPI.Data
{
    public class ApiService : IApiService
    {
        public IDictionary<string, IMovieRepository> MovieServices { get; } = new Dictionary<string, IMovieRepository>();

        public ApiService(HttpClient httpClient,
            ILogger<ApiService> logger,
            IOptions<CinemaServices> genericSettings,
            IMemoryCache memoryCache)
        {
            var lazyHttpClient = new Lazy<HttpClient>(() => httpClient);
            genericSettings.Value.Names.ForEach(n => MovieServices.Add(n, new MovieRepository(n, lazyHttpClient, logger, memoryCache)));
        }
    }
}