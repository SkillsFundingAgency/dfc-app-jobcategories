using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.ViewModels;
using FakeItEasy;
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
    public class PagesControllerBodyTests : BasePagesController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task PagesControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = new List<JobCategory>
            {
                new JobCategory()
                {
                    Title = "Care Worker",
                    CanonicalName = article,
                    JobProfiles = new List<JobProfile>()
                    {
                        new JobProfile()
                        {
                            Title = "Care Worker",
                            Description = "Job Profile",
                            Uri = new Uri("http://some.web.site/jobprofile/blah"),
                        },
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

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
            var expectedResult = new List<JobCategory>
            {
                new JobCategory()
                {
                    Title = "Care Worker",
                    CanonicalName = article,
                    JobProfiles = new List<JobProfile>()
                    {
                        new JobProfile()
                        {
                            Title = "Care Worker",
                            Description = "Job Profile",
                            Uri = new Uri("http://some.web.site/jobprofile/blah"),
                        },
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

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
            var expectedResult = new List<JobCategory>
            {
                new JobCategory()
                {
                    Title = "Care Worker",
                    CanonicalName = article,
                    JobProfiles = new List<JobProfile>()
                    {
                        new JobProfile()
                        {
                            Title = "Care Worker",
                            Description = "Job Profile",
                            Uri = new Uri("http://some.web.site/jobprofile/blah"),
                        },
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

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
            var expectedResult = new List<JobCategory>
            {
                new JobCategory()
                {
                    Title = "Care Worker",
                    CanonicalName = article,
                    JobProfiles = new List<JobProfile>()
                    {
                        new JobProfile()
                        {
                            Title = "Care Worker",
                            Description = "Job Profile",
                            Uri = new Uri("http://some.web.site/jobprofile/blah"),
                        },
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

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
            List<JobCategory>? expectedResult = null;
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

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
            var expectedResult = new List<JobCategory>
            {
                new JobCategory()
                {
                    Title = "Care Worker",
                    CanonicalName = article,
                    JobProfiles = new List<JobProfile>()
                    {
                        new JobProfile()
                        {
                            Title = "Care Worker",
                            Description = "Job Profile",
                            Uri = new Uri("http://some.web.site/jobprofile/blah"),
                        },
                    },
                },
            };
            var controller = BuildPagesController(mediaTypeName);

            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map(A<JobCategory>.Ignored, A<BodyViewModel>.Ignored)).Returns(A.Fake<BodyViewModel>());

            // Act
            var result = await controller.Body(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }
    }
}
