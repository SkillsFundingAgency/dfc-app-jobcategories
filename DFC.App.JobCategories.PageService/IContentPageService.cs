using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        Task<HttpStatusCode> DeleteAsync(Guid documentId);
        Task<bool> DeleteAllAsync();
        Task<IEnumerable<T?>> GetByQueryAsync(Expression<Func<T, bool>> where);
    }
}