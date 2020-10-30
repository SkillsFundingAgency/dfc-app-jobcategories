using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerPostTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerPostReturnsSuccessForCreate(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.Created;
            JobCategory? expectedResult = null;
            var contentPageModel = A.Fake<JobCategory>();
            contentPageModel.Id = Guid.NewGuid();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, null)).Returns(expectedResult);
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await controller.Create(contentPageModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerPostReturnsAlreadyReportedForCreate(string mediaTypeName)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.AlreadyReported;
            var existingModel = A.Fake<JobCategory>();
            var contentPageModel = A.Fake<JobCategory>();
            contentPageModel.Id = Guid.NewGuid();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, null)).Returns(existingModel);

            // Act
            var result = await controller.Create(contentPageModel).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetByIdAsync(A<Guid>.Ignored, null)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustNotHaveHappened();
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)expectedResponse, statusCodeResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerPostReturnsBadResultWhenModelIsNull(string mediaTypeName)
        {
            // Arrange
            JobCategory? contentPagesModel = null;
            var controller = BuildPagesController(mediaTypeName);

            // Act
            var result = await controller.Create(contentPagesModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerPostReturnsBadResultWhenModelIsInvalid(string mediaTypeName)
        {
            // Arrange
            var contentPagesModel = new JobCategory();
            var controller = BuildPagesController(mediaTypeName);

            controller.ModelState.AddModelError(string.Empty, "Model is not valid");

            // Act
            var result = await controller.Create(contentPagesModel).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
