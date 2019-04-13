using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using WebJetMoviesAPI.Core.Repository;

namespace WebJetMoviesAPI.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly string _endPointUrl;
        private readonly Lazy<HttpClient> _htClient;
        private readonly ILogger<ApiService> _logger;
        private IMemoryCache _cache;

        protected Repository(string endPointUrl,
            Lazy<HttpClient> htClient,
            ILogger<ApiService> logger,
            IMemoryCache memoryCache)
        {
            _endPointUrl = endPointUrl;
            _htClient = htClient;
            _logger = logger;
            _cache = memoryCache;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string methodUrl)
        {
            var allEntries = await
                _cache.GetOrCreateAsync(_endPointUrl, async (entry) =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    var requestUri = $"{_endPointUrl}/{methodUrl}/";

                    var response = await _htClient.Value.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content
                        .ReadAsAsync<IDictionary<string, IEnumerable<TEntity>>>();

                    _logger.LogDebug($"Successfully returned call from \"{requestUri}\"");

                    return result.Values.ElementAt(0);
                });

            return allEntries;
        }

        public async Task<TEntity> GetAsync(string methodUrl, string id)
        {
            var entity = await
                _cache.GetOrCreateAsync(id, async (entry) =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    var requestUri = $"{_endPointUrl}/{methodUrl}/{id}";
                    var response = await _htClient.Value.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsAsync<TEntity>();

                    _logger.LogDebug($"Successfully returned call from \"{requestUri}\"");

                    return result;
                });

            return entity;
        }

        public async Task<TEntity> FindAsync(string methodUrl, Func<TEntity, bool> filter)
        {
            var allEntries = await GetAllAsync(methodUrl);
            return allEntries.FirstOrDefault(filter);
        }
    }
}