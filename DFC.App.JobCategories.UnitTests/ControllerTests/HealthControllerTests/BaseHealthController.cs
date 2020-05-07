using DFC.App.JobCategories.Controllers;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.PageService;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.HealthControllerTests
{
    public class BaseHealthController
    {
        public BaseHealthController()
        {
            FakeContentPageService = A.Fake<IContentPageService<JobCategory>>();
            FakeLogger = A.Fake<ILogger<HealthController>>();
        }

        protected IContentPageService<JobCategory> FakeContentPageService { get; }

        protected ILogger<HealthController> FakeLogger { get; }

        protected HealthController BuildHealthController()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            var controller = new HealthController(FakeLogger, FakeContentPageService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
