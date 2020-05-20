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
    public class PagesControllerHtmlHeadTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobCategory>();
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobCategory>();
            var controller = BuildPagesController(mediaTypeName);

            expectedResult.CanonicalName = article;

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadWithNullArticleHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            JobCategory? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            JobCategory? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHtmlHeadHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            JobCategory? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHtmlHeadJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            JobCategory? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HtmlHeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerHtmlHeadReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobCategory>();
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).Returns(A.Fake<HtmlHeadViewModel>());

            // Act
            var result = await controller.HtmlHead(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobCategoryContentPageService.GetByCanonicalNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HtmlHeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
