using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public interface IApiDataService<TClientOptions>
        where TClientOptions : ServiceTaxonomyApiClientOptions
    {
        Task<IEnumerable<T>> GetAllAsync<T>(string contentType)
            where T : class;

        Task<T> GetByIdAsync<T>(string contentType, Guid id)
            where T : class;
    }
}
