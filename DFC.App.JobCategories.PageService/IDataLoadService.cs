using DFC.App.JobCategories.Data.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
