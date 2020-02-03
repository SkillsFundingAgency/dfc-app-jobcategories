using Dfc.App.JobCategories.Extensions;
using Dfc.App.JobCategories.Services;
using Dfc.App.JobCategories.ViewModels.Health;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogService logService;
        private readonly IJobCategoriesService jobCategoriesService;

        public HealthController(ILogService logService, IJobCategoriesService jobCategoriesService)
        {
            this.logService = logService;
            this.jobCategoriesService = jobCategoriesService;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            var resourceName = typeof(Program).Namespace;
            logService.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await jobCategoriesService.PingAsync().ConfigureAwait(false);
                if (isHealthy)
                {
                    const string message = "Document store is available";
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                logService.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(Health)}: {resourceName} exception: {ex.Message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            logService.LogVerbose($"{nameof(Ping)} has been called");
            return Ok();
        }

        private static HealthViewModel CreateHealthViewModel(string resourceName, string message)
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