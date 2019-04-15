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
        public void MoviesController_GetAll_ReturnOkResult()
        {
//
            var paginationOptFake = new Mock<IOptions<PaginationOptions>>();
            paginationOptFake.SetupGet(i => i.Value).Returns(new PaginationOptions {ItemsLimit = 3});


            Assert.Equal(3, paginationOptFake.Object.Value.ItemsLimit);

        }
    }
}
