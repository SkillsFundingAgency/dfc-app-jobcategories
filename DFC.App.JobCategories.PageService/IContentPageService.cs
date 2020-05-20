using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public interface IContentPageService<T>
        where T : class, IDataModel
    {
        Task<bool> PingAsync();

        Task<IEnumerable<T>?> GetAllAsync();

        Task<T?> GetByIdAsync(Guid documentId);

        Task<T?> GetByCanonicalNameAsync(string? canonicalName);

        Task<T?> GetByUriAsync(Uri? uri);

        Task<HttpStatusCode> UpsertAsync(T? contentPageModel);

        Task<bool> DeleteAsync(Guid documentId);
    }
}