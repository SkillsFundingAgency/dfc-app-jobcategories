using AutoMapper;
using DFC.App.JobCategories.Controllers;
using DFC.App.JobCategories.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Pages Controller Unit Tests")]
    public class PagesControllerRouteTests
    {
        private readonly ILogger<PagesController> logger;
        private readonly IDocumentService<JobCategory> fakeDocumentService;
        private readonly IMapper fakeMapper;

        public PagesControllerRouteTests()
        {
            logger = A.Fake<ILogger<PagesController>>();
            fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            fakeMapper = A.Fake<IMapper>();
        }

        public static IEnumerable<object[]> PagesRouteDataOk => new List<object[]>
        {
            new object[] { "/pages/{article}/htmlhead", "SomeArticle", nameof(PagesController.HtmlHead) },
            new object[] { "/pages/htmlhead", string.Empty, nameof(PagesController.HtmlHead) },
            new object[] { "/pages/{article}/breadcrumb", "SomeArticle", nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/breadcrumb", string.Empty, nameof(PagesController.Breadcrumb) },
            new object[] { "/pages/{article}/body", "SomeArticle", nameof(PagesController.Body) },
            new object[] { "/pages/body", string.Empty, nameof(PagesController.Body) },
        };

        public static IEnumerable<object[]> PagesRouteDataNoContent => new List<object[]>
        {
            new object[] { "/pages/{article}/bodytop", "SomeArticle", nameof(PagesController.BodyTop) },
            new object[] { "/pages/bodytop", string.Empty, nameof(PagesController.BodyTop) },
            new object[] { "/pages/{article}/herobanner", "SomeArticle", nameof(PagesController.HeroBanner) },
            new object[] { "/pages/herobanner", string.Empty, nameof(PagesController.HeroBanner) },
            new object[] { "/pages/{article}/sidebarright", "SomeArticle", nameof(PagesController.SidebarRight) },
            new object[] { "/pages/sidebarright", string.Empty, nameof(PagesController.SidebarRight) },
            new object[] { "/pages/{article}/sidebarleft", "SomeArticle", nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/sidebarleft", string.Empty, nameof(PagesController.SidebarLeft) },
            new object[] { "/pages/{article}/bodyfooter", "SomeArticle", nameof(PagesController.BodyFooter) },
            new object[] { "/pages/bodyfooter", string.Empty, nameof(PagesController.BodyFooter) },
        };

        [Theory]
        [MemberData(nameof(PagesRouteDataOk))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteForOkResult(string route, string article, string actionMethod)
        {
            // Arrange
            var controller = BuildController(route);
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

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // Act
            var result = await RunControllerAction(controller, article, actionMethod).ConfigureAwait(false);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PagesRouteDataNoContent))]
        public async Task PagesControllerCallsContentPageServiceUsingPagesRouteFornoContentResult(string route, string article, string actionMethod)
        {
            // Arrange
            var controller = BuildController(route);
            List<JobCategory>? expectedResult = null;

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // Act
            var result = await RunControllerAction(controller, article, actionMethod).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NoContentResult>(result);

            A.Equals((int)HttpStatusCode.NoContent, statusResult.StatusCode);

            controller.Dispose();
        }

        private static async Task<IActionResult> RunControllerAction(PagesController controller, string article, string actionName)
        {
            return actionName switch
            {
                nameof(PagesController.HtmlHead) => await controller.HtmlHead(article).ConfigureAwait(false),
                nameof(PagesController.Breadcrumb) => await controller.Breadcrumb(article).ConfigureAwait(false),
                nameof(PagesController.BodyTop) => controller.BodyTop(article),
                nameof(PagesController.HeroBanner) => controller.HeroBanner(article),
                nameof(PagesController.SidebarRight) => await controller.SidebarRight(article).ConfigureAwait(false),
                nameof(PagesController.SidebarLeft) => controller.SidebarLeft(article),
                nameof(PagesController.BodyFooter) => controller.BodyFooter(article),
                _ => await controller.Body(article).ConfigureAwait(false),
            };
        }

        private PagesController BuildController(string route)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = route;
            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            return new PagesController(logger, fakeDocumentService, fakeMapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };
        }
    }
}