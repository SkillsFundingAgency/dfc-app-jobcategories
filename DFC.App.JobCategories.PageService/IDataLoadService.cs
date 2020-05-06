using DFC.App.JobCategories.Data.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public interface IDataLoadService<TClientOptions>
        where TClientOptions : ServiceTaxonomyApiClientOptions
    {
        Task<string> GetAllAsync(string contentType);

        Task<string> GetByIdAsync(string contentType, Guid id);
    }
}
