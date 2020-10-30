using DFC.App.JobCategories.Controllers;
using DFC.App.JobCategories.Data.Models;
using DFC.Compui.Cosmos.Contracts;
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
            FakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            FakeLogger = A.Fake<ILogger<HealthController>>();
        }

        protected IDocumentService<JobCategory> FakeDocumentService { get; }

        protected ILogger<HealthController> FakeLogger { get; }

        protected HealthController BuildHealthController()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = MediaTypeNames.Application.Json;

            var controller = new HealthController(FakeLogger, FakeDocumentService)
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
