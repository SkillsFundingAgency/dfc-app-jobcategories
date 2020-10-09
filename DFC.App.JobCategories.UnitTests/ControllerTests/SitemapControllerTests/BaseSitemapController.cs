using DFC.App.JobCategories.Controllers;
using DFC.App.JobCategories.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobCategories.UnitTests.ControllerTests.SitemapControllerTests
{
    public class BaseSitemapController
    {
        public BaseSitemapController()
        {
            FakeLogger = A.Fake<ILogger<SitemapController>>();
            FakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
        }

        protected ILogger<SitemapController> FakeLogger { get; }

        protected IDocumentService<JobCategory> FakeDocumentService { get; }

        protected SitemapController BuildSitemapController()
        {
            var controller = new SitemapController(FakeLogger, FakeDocumentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            return controller;
        }
    }
}
