using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerExploreCareersTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerExploreCareersHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildPagesController(mediaTypeName);
            IEnumerable<JobCategory>? expectedResult = new List<JobCategory>();

            A.CallTo(() => FakeDocumentService.GetAllAsync(null)).Returns(expectedResult);

            // Act
            var result = await controller.ExploreCareers().ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<ViewResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);
            Assert.IsAssignableFrom<IEnumerable<JobCategory>>(statusResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerExploreCareersJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildPagesController(mediaTypeName);
            IEnumerable<JobCategory>? expectedResult = new List<JobCategory>();

            A.CallTo(() => FakeDocumentService.GetAllAsync(null)).Returns(expectedResult);

            // Act
            var result = await controller.ExploreCareers().ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<OkObjectResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);
            Assert.IsAssignableFrom<IEnumerable<JobCategory>>(statusResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerSidebarRightWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var controller = BuildPagesController(mediaTypeName);
            IEnumerable<JobCategory>? expectedResult = null;

            A.CallTo(() => FakeDocumentService.GetAllAsync(null)).Returns(expectedResult);

            // Act
            var result = await controller.ExploreCareers().ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
