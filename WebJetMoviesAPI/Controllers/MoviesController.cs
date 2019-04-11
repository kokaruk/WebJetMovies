using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebJetMoviesAPI.Core;

namespace WebJetMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IApiService _apiService;

        public MoviesController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get() =>
            Ok(await _apiService.Filmworld.GetAllAsync("movies"));

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }
    }
}