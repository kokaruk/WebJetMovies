using System.Threading.Tasks;

namespace WebJetMoviesAPI.Core
{
    public interface IPosterService
    {
        Task<string> GetAsync(string movieTitle, string year);
    }
}