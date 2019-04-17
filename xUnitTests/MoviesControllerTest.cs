using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
                {new Movie {Id = "1", Title = "T1"}, new Movie {Id = "2", Title = "T2"}};
            var moqMovieRepository = new Mock<IMovieRepository>();
            moqMovieRepository.Setup(rep => rep.GetAllAsync(endPoint)).ReturnsAsync(fakeMoviesList);

            var movieRepoFake = new Dictionary<string, IMovieRepository> {{"rep", moqMovieRepository.Object}};


            var apiServiceMoq = new Mock<IApiService>();
            var paginationOptionsStub = new PaginationOptions();

            apiServiceMoq.SetupGet(ms => ms.MovieServices).Returns(movieRepoFake);

            var posterServiceMoq = new Mock<IPosterService>();
            var optionsMoq = new Mock<IOptions<PaginationOptions>>();
            optionsMoq.SetupGet(i => i.Value).Returns(paginationOptionsStub);


            var moviesController =
                new MoviesController(apiServiceMoq.Object, posterServiceMoq.Object, optionsMoq.Object);

//            // act
            var result = await moviesController.GetAll();

//            //assert
            Assert.IsType<OkObjectResult>(result.Result);

            var response =
                Assert.IsAssignableFrom<PageCollectionResponse<Movie>>(((OkObjectResult) result.Result).Value);
            
            Assert.Equal(2, response.Items.Count());
        }

        [Fact]
        public void MoviesController_GetAll_ReturnOkResultPaginated()
        {
        }

        [Fact]
        public void MoviesController_GetAll_ReturnNoContentNoMoviesForAllRequest()
        {
        }

        [Fact]
        public void MoviesController_GetAll_ReturnNoContentNoMoviesPaginated()
        {
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