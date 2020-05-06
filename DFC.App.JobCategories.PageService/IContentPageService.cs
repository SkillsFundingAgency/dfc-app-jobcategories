using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public interface IContentPageService
    {
        Task<bool> PingAsync();

        Task<IEnumerable<JobCategory>?> GetAllAsync();

        Task<JobCategory?> GetByIdAsync(Guid documentId);

        Task<JobCategory?> GetByCanonicalNameAsync(string? canonicalName);

        Task<HttpStatusCode> UpsertAsync(JobCategory? contentPageModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}