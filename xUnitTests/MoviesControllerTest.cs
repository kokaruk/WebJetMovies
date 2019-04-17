using System;
using System.Collections.Generic;
using System.Linq;
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

        public MoviesControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnOkResultOfFullCollection()
        {
            // arrange
            const string endPoint = "movies";

            var fakeMoviesList = new List<Movie>
            {
                new Movie {Id = "1", Title = "T1"}, 
                new Movie {Id = "2", Title = "T2"}
            };
            
            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(repo => repo.GetAllAsync(endPoint)).ReturnsAsync(fakeMoviesList);

            var movieRepoDictFake = new Dictionary<string, IMovieRepository> {{"rep", moqMovieRepository.Object}};
            var apiServiceMoq = new Mock<IApiService>();
            var paginationOptionsStub = new PaginationOptions();

            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(movieRepoDictFake);

            var posterServiceMoq = new Mock<IPosterService>();
            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(paginationOptionsStub);

            var moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object);

            // act
            var result = await moviesController.GetAll();

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
            const string endPoint = "movies";

            var fakeMoviesList = new List<Movie>
            {
                new Movie {Id = "1", Title = "T1"}, 
                new Movie {Id = "2", Title = "T2"}
            };
            
            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(rep => rep.GetAllAsync(endPoint)).ReturnsAsync(fakeMoviesList);

            var movieRepoDictFake = new Dictionary<string, IMovieRepository> {{"rep", moqMovieRepository.Object}};

            var apiServiceMoq = new Mock<IApiService>();
            var paginationOptionsStub = new PaginationOptions {ItemsLimit = 1};

            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(movieRepoDictFake);

            var posterServiceMoq = new Mock<IPosterService>();
            posterServiceMoq.Setup(p => p.GetAsync("some title", "some year"))
                .ReturnsAsync("fake url");
            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(paginationOptionsStub);

            var urlHelperMoq = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMoq
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("https://staging.kokaruk.com/")
                .Verifiable();

            var moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object)
                {
                    Url = urlHelperMoq.Object, ControllerContext = {HttpContext = new DefaultHttpContext()}
                };

            // act
            var result1 = await moviesController.GetAll(page: 1);
            var result2 = await moviesController.GetAll(page: 2);

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
        public async void MoviesController_GetAll_ReturnNoContentNoMoviesForAllRequest()
        {
            // arrange
            const string endPoint = "movies";

            // ReSharper disable once CollectionNeverUpdated.Local
            var fakeMoviesList = new List<Movie>();
            
            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(repo => repo.GetAllAsync(endPoint)).ReturnsAsync(fakeMoviesList);

            var movieRepoDictFake = new Dictionary<string, IMovieRepository> {{"rep", moqMovieRepository.Object}};
            var apiServiceMoq = new Mock<IApiService>();
            var paginationOptionsStub = new PaginationOptions();

            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(movieRepoDictFake);

            var posterServiceMoq = new Mock<IPosterService>();
            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(paginationOptionsStub);

            var moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object);

            // act
            var result = await moviesController.GetAll();

            //assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async void MoviesController_GetAll_ReturnNoContentNoMoviesPaginated()
        {
            // arrange
            const string endPoint = "movies";

            var fakeMoviesList = new List<Movie>
            {
                new Movie {Id = "1", Title = "T1"}, 
                new Movie {Id = "2", Title = "T2"}
            };
            
            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(rep => rep.GetAllAsync(endPoint)).ReturnsAsync(fakeMoviesList);

            var movieRepoDictFake = new Dictionary<string, IMovieRepository> {{"rep", moqMovieRepository.Object}};

            var apiServiceMoq = new Mock<IApiService>();
            var paginationOptionsStub = new PaginationOptions {ItemsLimit = 1};

            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(movieRepoDictFake);

            var posterServiceMoq = new Mock<IPosterService>();
            posterServiceMoq.Setup(p => p.GetAsync("some title", "some year"))
                .ReturnsAsync("fake url");
            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(paginationOptionsStub);

            var urlHelperMoq = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMoq
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("https://staging.kokaruk.com/")
                .Verifiable();

            var moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object)
                {
                    Url = urlHelperMoq.Object, ControllerContext = {HttpContext = new DefaultHttpContext()}
                };

            // act
            var result = await moviesController.GetAll(page: 3);
            
            // assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public void MoviesController_Get_ReturnOkResult()
        {
        }

        [Fact]
        public void MoviesController_Get_ReturnBadRequestForEmptyArguments()
        {
        }

        [Fact]
        public void MoviesController_Get_ReturnNoContentWhenNoMIvieFound()
        {
        }
    }
}