using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using WebJetMoviesAPI.Controllers;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Core.Repository;
using WebJetMoviesAPI.Models;
using WebJetMoviesAPI.Utils;
using WebJetMoviesAPI.Utils.SettingsModels;
using Xunit.Abstractions;


namespace TestProject1
{
    public class MoviesControllerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly List<Movie> _fakeMoviesList;
        private readonly PaginationOptions _paginationOptionsStub;
        private readonly MoviesController _moviesController;
        private readonly Mock<IMovieRepository> _moqMovieRepository;

        private readonly Dictionary<string, IMovieRepository> _movieRepoDictFake;

        private const string CollectionEndPoint = "movies";
        private const string SingleEndpoint = "movie";

        private readonly Movie _fakeMovie;

        public MoviesControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fakeMovie = new Movie {Id = "A1", Title = "Real Fake Title", Price = 10, Year = "2222"};

            _fakeMoviesList = new List<Movie>
            {
                _fakeMovie,
                new Movie {Id = "A2", Title = "Real Fake Title Other", Price = 1, Year = "2222"}
            };

            _moqMovieRepository = new Mock<IMovieRepository>();
            _moqMovieRepository.Setup(repo =>
                    repo.GetAllAsync(CollectionEndPoint))
                .ReturnsAsync(_fakeMoviesList);

            _movieRepoDictFake = new Dictionary<string, IMovieRepository>
                {{"movieRepKey", _moqMovieRepository.Object}};

            var apiServiceMoq = new Mock<IApiService>();
            _paginationOptionsStub = new PaginationOptions();
            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(_movieRepoDictFake);

            var posterServiceMoq = new Mock<IPosterService>();
            posterServiceMoq.Setup(p => p.GetAsync(_fakeMovie.Title, "2222"))
                .ReturnsAsync("fake url");

            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(_paginationOptionsStub);
            var urlHelperMoq = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMoq
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("https://staging.kokaruk.com/")
                .Verifiable();

            _moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object)
                {
                    Url = urlHelperMoq.Object, ControllerContext = {HttpContext = new DefaultHttpContext()}
                };
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnOkResultOfFullCollection()
        {
            // arrange

            // act
            var result = await _moviesController.GetAll();

            //assert
            Assert.IsType<OkObjectResult>(result.Result);

            var response =
                Assert.IsAssignableFrom<PageCollectionResponse<Movie>>(((OkObjectResult) result.Result).Value);

            Assert.Equal(2, response.Items.Count());

            Assert.Null(response.NextPage);
            Assert.Null(response.PreviousPage);
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnOkResultPaginated()
        {
            // arrange
            _paginationOptionsStub.ItemsLimit = 1;
            // act
            var result1 = await _moviesController.GetAll(page: 1);
            var result2 = await _moviesController.GetAll(page: 2);
            // assert
            Assert.IsType<OkObjectResult>(result1.Result);
            Assert.IsType<OkObjectResult>(result2.Result);

            var response1 =
                Assert.IsAssignableFrom<PageCollectionResponse<Movie>>(((OkObjectResult) result1.Result).Value);
            var response2 =
                Assert.IsAssignableFrom<PageCollectionResponse<Movie>>(((OkObjectResult) result2.Result).Value);

            Assert.Single(response1.Items);
            Assert.Single(response2.Items);

            Assert.Null(response1.PreviousPage);
            Assert.NotNull(response1.NextPage);
            Assert.NotNull(response2.PreviousPage);
            Assert.Null(response2.NextPage);
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnNoContentNoMoviesForAllRequests()
        {
            // arrange
            _fakeMoviesList.Clear();
            // act
            var result = await _moviesController.GetAll();
            //assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnNoContentNoMoviesPaginated()
        {
            // arrange

            // act
            var result = await _moviesController.GetAll(page: 3);

            // assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Theory]
        [InlineData(3, 3, "movieRepKey2")]
        [InlineData(15, 10, "movieRepKey")]
        public async void MoviesController_Get_ReturnOkResult(int price, int expectedPrice, string expectedCollection)
        {
            // arrange
            var fakeMovie = new Movie {Id = "B1", Title = "Real Fake Title", Price = price, Year = "2222"};

            var fakeMoviesList = new List<Movie> {fakeMovie};

            _moqMovieRepository.Setup(repo =>
                    repo.FindAsync(CollectionEndPoint, It.IsAny<Func<Movie, bool>>()))
                .ReturnsAsync(_fakeMovie);
            _moqMovieRepository.Setup(repo =>
                    repo.GetAsync(SingleEndpoint, _fakeMovie.Id))
                .ReturnsAsync(_fakeMovie);

            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(repo =>
                    repo.GetAllAsync(CollectionEndPoint))
                .ReturnsAsync(fakeMoviesList);


            moqMovieRepository.Setup(repo =>
                    repo.FindAsync(CollectionEndPoint, It.IsAny<Func<Movie, bool>>()))
                .ReturnsAsync(fakeMovie);

            moqMovieRepository.Setup(repo =>
                    repo.GetAsync(SingleEndpoint, fakeMovie.Id))
                .ReturnsAsync(fakeMovie);

            _movieRepoDictFake.Add("movieRepKey2", moqMovieRepository.Object);

            // act
            var result = await _moviesController.Get(year: "2222",
                title: "Real Fake Title");

            var value = (TupleWrapperResponse<Movie>) ((OkObjectResult) result.Result).Value;

            // assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedPrice, value.Member.Price);
            Assert.Equal(expectedCollection, value.ParentName);
        }

        [Fact]
        public async void MoviesController_Get_ReturnBadRequestForEmptyArguments()
        {
            // arrange

            // act
            var result1 = await _moviesController.Get(year: "someYearAsString",
                title: "someFakeTitle");
            var result2 = await _moviesController.Get(year: "",
                title: "someFakeTitle");
            var result3 = await _moviesController.Get(year: "222",
                title: "someFakeTitle");

            // assert
            Assert.IsType<BadRequestResult>(result1.Result);
            Assert.IsType<BadRequestResult>(result2.Result);
            Assert.IsType<BadRequestResult>(result3.Result);
        }

        [Fact]
        public async void MoviesController_Get_ReturnNoContentWhenNoMIvieFound()
        {
            // arrange

            // act
            var result = await _moviesController.Get(year: "1111",
                title: "some Fake Title");

            // assert
            Assert.IsType<NoContentResult>(result.Result);
        }
    }
}