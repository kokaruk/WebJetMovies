using System.Collections.Generic;
using WebJetMoviesAPI.Core.Repository;

namespace WebJetMoviesAPI.Core
{
    public interface IApiService
    {
        IDictionary<string, IMovieRepository> MovieServices { get; }
    }
}