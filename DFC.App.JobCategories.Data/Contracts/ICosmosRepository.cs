using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Data.Contracts
{
    public interface ICosmosRepository<T>
        where T : class, IDocumentModel
    {
        Task<bool> PingAsync();

        Task<T?> GetAsync(Expression<Func<T, bool>> where);

        Task<IEnumerable<T?>?> GetListAsync(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>?> GetAllAsync();

        Task<HttpStatusCode> UpsertAsync(T model);

        Task<HttpStatusCode> DeleteAsync(Guid documentId);

        Task<HttpStatusCode> DeleteAllAsync();
    }
}