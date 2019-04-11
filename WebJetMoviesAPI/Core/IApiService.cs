using System;
using System.Threading.Tasks;
using WebJetMoviesAPI.Core.Repository;

namespace WebJetMoviesAPI.Core
{
    public interface IApiService
    {
        IMovieRepository Cinemaworld { get; }
        IMovieRepository Filmworld { get; }
    }
}