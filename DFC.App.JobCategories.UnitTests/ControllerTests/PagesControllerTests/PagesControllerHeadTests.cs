using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerHeadTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new List<JobCategory> { new JobCategory() { Title = "Care Worker", CanonicalName = article } };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new List<JobCategory> { new JobCategory() { Title = "Care Worker", CanonicalName = article } };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().NotBeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadWithNullArticleHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            List<JobCategory>? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadHtmlWithNullArticleJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string? article = null;
            List<JobCategory>? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            _ = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerHeadHtmlReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            List<JobCategory>? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task PagesControllerHeadJsonReturnsSuccessWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            List<JobCategory>? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(jsonResult.Value);

            model.CanonicalUrl.Should().BeNull();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task PagesControllerHeadReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new List<JobCategory> { new JobCategory() { Title = "Care Worker", CanonicalName = article } };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).Returns(A.Fake<HeadViewModel>());

            // Act
            var result = await controller.Head(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<HeadViewModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
