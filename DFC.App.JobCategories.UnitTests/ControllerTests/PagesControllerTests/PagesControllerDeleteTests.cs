using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerDeleteTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerDeleteReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.NoContent);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var okResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerDeleteReturnsNotFound(string mediaTypeName)
        {
            // Arrange
            Guid documentId = Guid.NewGuid();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.Delete(documentId).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
