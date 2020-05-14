using DFC.App.JobCategories.Data.Models;
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
    public class ApiDataService<TClientOptions> : IApiDataService<TClientOptions>
        where TClientOptions : ServiceTaxonomyApiClientOptions
    {
        private readonly HttpClient httpClient;
        private readonly TClientOptions serviceTaxonomyClientOptions;

        public ApiDataService(HttpClient httpClient, TClientOptions serviceTaxonomyClientOptions)
        {
            this.httpClient = httpClient;
            this.serviceTaxonomyClientOptions = serviceTaxonomyClientOptions;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string contentType)
            where T : class
        {
            var result = await GetAsync(contentType, null).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<T>>(result);
        }

        public async Task<T> GetByIdAsync<T>(string contentType, Guid id)
            where T : class
        {
            var result = await GetAsync(contentType, id).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(result);
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
            catch (BrokenCircuitException)
            {
                return string.Empty;
            }
        }
    }
}
