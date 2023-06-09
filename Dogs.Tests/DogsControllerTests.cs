using Dogs.Controllers;
using Dogs.DataAccess.Data;
using Dogs.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace Dogs.Tests
{
    public class DogsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;

        public DogsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            SeedDatabase();
        }


        [Fact]
        public async Task Ping_ReturnsOkResultWithApiResponse()
        {
            // Arrange
            var controller = new DogsController(_dbContext);

            // Act
            var result =  controller.Ping();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsType<ApiResponse>(okResult.Value);
            var response = okResult.Value as ApiResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Dogs house service.Version 1.0.1", response.Result);
        }

        [Fact]
        public async Task GetDogs_WithNoSorting_ReturnsOkResultWithApiResponse()
        {
            // Arrange
            var controller = new DogsController(_dbContext);

            // Act
            var result = await controller.GetDogs();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsType<ApiResponse>(okResult.Value);
            var response = okResult.Value as ApiResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<List<Dog>>(response.Result);
            var dogs = response.Result as List<Dog>;
            Assert.Equal(2, dogs.Count);
        }

        [Fact]
        public async Task GetDogs_WithSorting_ReturnsOkResultWithSortedApiResponse()
        {
            // Arrange
            var controller = new DogsController(_dbContext);

            // Act
            var result = await controller.GetDogs(attribute: "Name", order: "asc");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsType<ApiResponse>(okResult.Value);
            var response = okResult.Value as ApiResponse;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<List<Dog>>(response.Result);
            var dogs = response.Result as List<Dog>;
            Assert.Equal("Jessy", dogs[0].Name);
            Assert.Equal("Max", dogs[1].Name);
        }

        [Fact]
        public async Task CreateDog_WithValidData_ReturnsOkResultWithApiResponse()
        {
            // Arrange
            var dogsController = new DogsController(_dbContext);
            var validDog = new Dog { Name = "Valid Dog", Color = "Valid Color", TailLength = 10, Weight = 20 };

            // Act
            var result = await dogsController.CreateDog(validDog);

            // Assert
            var okResult = Assert.IsType<ActionResult<ApiResponse>>(result);
            Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(((OkObjectResult)okResult.Result).Value);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.True(apiResponse.IsSuccess);
            Assert.Empty(apiResponse.ErrorMessages);
        }


        [Fact]
        public async Task CreateDog_WithInvalidData_ReturnsBadRequestResultWithErrorResponse()
        {
            // Arrange
            var dogsController = new DogsController(_dbContext);
            var invalidDog = new Dog { Name = "Test", Color = "Test", TailLength = -1, Weight = -1 };

            // Act
            var result = await dogsController.CreateDog(invalidDog);

            // Assert
            var badRequestResult = Assert.IsType<ActionResult<ApiResponse>>(result);
            Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(((BadRequestObjectResult)badRequestResult.Result).Value);
            Assert.Equal(HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.False(apiResponse.IsSuccess);
            Assert.NotNull(apiResponse.ErrorMessages);
            Assert.Contains("Can not create a dog!", apiResponse.ErrorMessages);
        }
  

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private void SeedDatabase()
        {
            _dbContext.Dogs.AddRange(
                new Dog
                {
                    Id = 1,
                    Name = "Max",
                    Color = "brown",
                    TailLength = 10,
                    Weight = 20
                },
                new Dog
                {
                    Id = 2,
                    Name = "Jessy",
                    Color = "black & white",
                    TailLength = 7,
                    Weight = 14
                });

            _dbContext.SaveChanges();
        }

    }
}
