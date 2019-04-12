using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebJetMoviesAPI.Core.Repository;

namespace WebJetMoviesAPI.Core
{
    public interface IApiService
    {
        IDictionary<string, IMovieRepository>  MovieServices { get; }
    }
}