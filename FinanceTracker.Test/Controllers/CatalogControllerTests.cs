using FinanceTracker.Server.Controllers;
using FinanceTracker.Server.Models.Entities;
using FinanceTracker.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.Test.Controllers
{
    public class CatalogControllerTests
    {
        private readonly Mock<ICatalogRepository> _mockCatalogRepository;
        private readonly CatalogController _catalogController;
        private readonly List<Category> categories;

        public CatalogControllerTests()
        {
            _mockCatalogRepository = new Mock<ICatalogRepository>();
            _catalogController = new CatalogController(_mockCatalogRepository.Object);

            categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Food" },
                new Category { Id = Guid.NewGuid(), Name = "Transport" },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment" }
            };
        }

        [Fact]
        public async Task GetCategories_ReturnsCategories()
        {
            // Arrange
            _mockCatalogRepository.Setup(repo => repo.GetCategories())
                .ReturnsAsync(categories);

            // Act
            var result = await _catalogController.GetCategories();

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);

            Assert.Equal(categories.Count, returnedCategories.Count());
            Assert.Equal(categories[0].Name, returnedCategories.First().Name);
            Assert.Equal(categories.Select(c => c.Id), returnedCategories.Select(c => c.Id));

            _mockCatalogRepository.Verify(repo => repo.GetCategories(), Times.Once);
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyList()
        {
            // Arrange
            _mockCatalogRepository.Setup(repo => repo.GetCategories())
                .ReturnsAsync(new List<Category>());

            // Act
            var result = await _catalogController.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);

            Assert.NotNull(returnedCategories);
            Assert.Empty(returnedCategories);

            _mockCatalogRepository.Verify(repo => repo.GetCategories(), Times.Once);
        }
    }
}
