using DFC.App.JobCategories.Data.Models;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.DataLoadService
{
    public class DataLoadService<TClientOptions> : IDataLoadService<TClientOptions>
        where TClientOptions : ServiceTaxonomyApiClientOptions
    {
        private readonly HttpClient httpClient;
        private readonly ILogService logService;
        private readonly TClientOptions serviceTaxonomyClientOptions;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public DataLoadService(HttpClient httpClient, ILogService logService, TClientOptions serviceTaxonomyClientOptions, ICorrelationIdProvider correlationIdProvider)
        {
            this.httpClient = httpClient;
            this.logService = logService;
            this.serviceTaxonomyClientOptions = serviceTaxonomyClientOptions;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task<string> GetAllAsync(string contentType)
        {
            return await GetAsync(contentType, null).ConfigureAwait(false);
        }

        public async Task<string> GetByIdAsync(string contentType, Guid id)
        {
            return await GetAsync(contentType, id).ConfigureAwait(false);
        }

        private async Task<string> GetAsync(string contentType, Guid? id)
        {
            var endpoint = string.Format(CultureInfo.InvariantCulture, serviceTaxonomyClientOptions.Endpoint, contentType, id.HasValue ? id.ToString() : string.Empty);
            var url = $"{serviceTaxonomyClientOptions.BaseAddress}{endpoint}";

            logService.LogInformation($"{nameof(GetAsync)}: Loading data segment from {url}");

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            ConfigureHttpClient();

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    logService.LogError($"Failed to get data for {contentType} :: {id} from {url}, received error : '{responseString}', Returning empty content.");
                    responseString = string.Empty;
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logService.LogInformation($"Status - {response.StatusCode} with response '{responseString}' received for {contentType} :: {id} from {url}, Returning empty content.");
                    responseString = string.Empty;
                }

                return responseString;
            }
            catch (BrokenCircuitException e)
            {
                logService.LogInformation($"Error received refreshing segment data '{e.InnerException?.Message}'. Received for {contentType} :: {id} from {url}, Returning empty content.");
                return string.Empty;
            }
        }

        private void ConfigureHttpClient()
        {
            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
            }
        }
    }
}
