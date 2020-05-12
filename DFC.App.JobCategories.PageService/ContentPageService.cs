using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public class ContentPageService<T> : IContentPageService<T>
        where T : class, IDataModel
    {
        private readonly ICosmosRepository<T> repository;

        public ContentPageService(ICosmosRepository<T> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>?> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<T?> GetByCanonicalNameAsync(string? canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T?>> GetByQueryAsync(Expression<Func<T, bool>> where)
        {
            return await repository.GetListAsync(where).ConfigureAwait(false);
        }

        public async Task<T?> GetByUriAsync(Uri? uri)
        {
            return await repository.GetAsync(d => d.Uri == uri).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> UpsertAsync(T? contentPageModel)
        {
            if (contentPageModel == null)
            {
                throw new ArgumentNullException(nameof(contentPageModel));
            }

            return await repository.UpsertAsync(contentPageModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var result = await repository.DeleteAllAsync().ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}