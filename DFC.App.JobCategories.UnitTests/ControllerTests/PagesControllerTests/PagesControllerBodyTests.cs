using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerBodyTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new JobCategory() { Title = "Care Worker" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new JobCategory() { Title = "Care Worker" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string?>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<JobCategory>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyWithNullArticleHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            var expectedResult = new JobCategory() { Title = "Care Worker" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerBodyWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            var expectedResult = new JobCategory() { Title = "Care Worker" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<JobCategory>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyReturnsNotFoundWhenNoAlternateArticle(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            JobCategory? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string?>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<NotFoundResult>(result);

            A.Equals((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new JobCategory() { Title = "Care Worker" };
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string?>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
