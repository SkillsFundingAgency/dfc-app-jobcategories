using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.Extensions
{
    public class ApiExtensions : IApiExtensions
    {
        private readonly IApiDataService<ServiceTaxonomyApiClientOptions> apiDataService;

        public ApiExtensions(IApiDataService<ServiceTaxonomyApiClientOptions> apiDataService)
        {
            this.apiDataService = apiDataService;
        }

        public async Task<IEnumerable<T>> LoadDataAsync<T>(string contentType)
           where T : class
        {
            var data = await apiDataService.GetAllAsync<T>(contentType).ConfigureAwait(false);
            return data;
        }

        public async Task<T> LoadDataByIdAsync<T>(string contentType, Guid id)
           where T : class
        {
            var data = await apiDataService.GetByIdAsync<T>(contentType, id).ConfigureAwait(false);
            return data;
        }
    }
}
