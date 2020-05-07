using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public class DataLoadService<TClientOptions> : IDataLoadService<TClientOptions>
        where TClientOptions : ServiceTaxonomyApiClientOptions
    {
        private readonly HttpClient httpClient;
        private readonly TClientOptions serviceTaxonomyClientOptions;

        public DataLoadService(HttpClient httpClient, TClientOptions serviceTaxonomyClientOptions)
        {
            this.httpClient = httpClient;
            this.serviceTaxonomyClientOptions = serviceTaxonomyClientOptions;
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

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    responseString = string.Empty;
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    responseString = string.Empty;
                }

                return responseString;
            }
            catch (BrokenCircuitException e)
            {
                return string.Empty;
            }
        }
    }
}
