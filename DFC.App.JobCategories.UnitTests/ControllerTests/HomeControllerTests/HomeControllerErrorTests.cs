using DFC.App.JobCategories.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.HomeControllerTests
{
    [Trait("Category", "Home Controller Unit Tests")]
    public class HomeControllerErrorTests : BaseHomeController
    {
        [Fact]
        public void HomeControllerErrorTestsReturnsSuccess()
        {
            // Arrange
            var controller = BuildHomeController();

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }
    }
}
