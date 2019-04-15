using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using WebJetMoviesAPI.Controllers;
using WebJetMoviesAPI.Core;
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
        public void MoviesController_GetAll_ReturnOkResultOfFullCollection()
        {
            var paginationOptFake = new Mock<IOptions<PaginationOptions>>();
            paginationOptFake.SetupGet(i => i.Value).Returns(new PaginationOptions {ItemsLimit = 3});


            Assert.False(true);
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
        public void MoviesController_Get_ReturnBadRequesForEmptyArguments()
        {
        }

        [Fact]
        public void MoviesController_Get_ReturnNoContentWhenNoMIvieFound()
        {
        }
    }
}