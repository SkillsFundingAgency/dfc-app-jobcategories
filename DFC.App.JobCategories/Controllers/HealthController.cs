using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Extensions;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;
        private readonly IContentPageService<JobCategory> contentPageService;
        private readonly string resourceName = typeof(Program).Namespace!;

        public HealthController(ILogger<HealthController> logger, IContentPageService<JobCategory> contentPageService)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            logger.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await contentPageService.PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    const string message = "Document store is available";
                    logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                logger.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(Health)}: {resourceName} exception: {ex.Message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private HealthViewModel CreateHealthViewModel(string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}