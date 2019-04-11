using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        
        protected Repository(string endPointUrl, Lazy<HttpClient> htClient, ILogger<ApiService> logger)
        {
            _endPointUrl = endPointUrl;
            _htClient = htClient;
            _logger = logger;

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string methodUrl)
        {
            var requestUri =$"{_endPointUrl}/{methodUrl}/";
            
            var response = await _htClient.Value.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadAsAsync<IDictionary<string,IEnumerable<TEntity>>>();
            
            _logger.LogDebug($"Successfully returned call from \"{requestUri}\"");
            
            return result.Values.ElementAt(0);
        }

        public async Task<TEntity> GetAsync(string id)
        {
            var response = await _htClient.Value.GetAsync(_endPointUrl + @"/" + id);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<TEntity>();

            return result;
        }
    }
}