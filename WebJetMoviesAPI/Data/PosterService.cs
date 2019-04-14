using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Utils.SettingsModels;
// ReSharper disable ClassNeverInstantiated.Global

namespace WebJetMoviesAPI.Data
{
    /// <summary>
    ///     separate service for poster retrieval
    ///     very simple design, in real world one would expect to receive correct URI from API supplier
    /// </summary>
    public class PosterService : IPosterService
    {
        private const int CacheLifeTime = 180;
        private readonly IMemoryCache _cache;
        private readonly Lazy<HttpClient> _htClient;
        private readonly ILogger<PosterService> _logger;
        private readonly IOptions<PosterServiceOptions> _posterServiceSettings;

        public PosterService(HttpClient httpClient,
            ILogger<PosterService> logger,
            IOptions<PosterServiceOptions> posterServiceSettings,
            IMemoryCache memoryCache)
        {
            _htClient = new Lazy<HttpClient>(() => httpClient);
            _logger = logger;
            _posterServiceSettings = posterServiceSettings;
            _cache = memoryCache;
        }

        public async Task<string> GetAsync(string movieTitle, string year)
        {
            var id = movieTitle + year;

            var entity = await
                _cache.GetOrCreateAsync(id, async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(CacheLifeTime);
                    var builder = new UriBuilder("https://api.themoviedb.org/3/search/movie") {Port = -1};
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    query["api_key"] = _posterServiceSettings.Value.ApiKey;
                    query["language"] = _posterServiceSettings.Value.language;
                    query["query"] = movieTitle;
                    query["page"] = _posterServiceSettings.Value.page;
                    query["include_adult"] = _posterServiceSettings.Value.include_adult;
                    query["year"] = year;

                    builder.Query = query.ToString();
                    var url = builder.ToString();
                    var response = await _htClient.Value.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsAsync<IDictionary<string, dynamic>>();
                    var posterPath = ((JArray) result["results"])
                        .Children<JObject>()
                        .Select(i => i.GetValue("poster_path"))
                        .FirstOrDefault(ii => ii != null);

                    return posterPath != null
                        ? _posterServiceSettings.Value.BaseImageUrl + posterPath.ToString()
                        : string.Empty;
                });

            return entity;
        }
    }
}