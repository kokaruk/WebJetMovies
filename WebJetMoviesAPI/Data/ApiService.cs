using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Data.Repository;
using WebJetMoviesAPI.Utils.SettingsModels;
// ReSharper disable ClassNeverInstantiated.Global

namespace WebJetMoviesAPI.Data
{
    /// <summary>
    ///     API service based on repository pattern
    /// </summary>
    public class ApiService : IApiService
    {
        public ApiService(HttpClient httpClient,
            ILogger<ApiService> logger,
            IOptions<CinemaServicesOptions> genericSettings,
            IMemoryCache memoryCache)
        {
            var lazyHttpClient = new Lazy<HttpClient>(() => httpClient);
            genericSettings.Value.Names.ForEach(n =>
                MovieServices.Add(n, new MovieRepository(n, lazyHttpClient, logger, memoryCache)));
        }

        public IDictionary<string, IMovieRepository> MovieServices { get; } =
            new Dictionary<string, IMovieRepository>();
    }
}