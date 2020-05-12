using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.EventProcessorServices
{
    public interface IEventMessageService
    {
        Task<IList<T>?> GetAllCachedCanonicalNamesAsync<T>()
            where T : IDataModel;

        Task<HttpStatusCode> CreateAsync<T>(T upsertContentPageModel)
            where T : IDataModel;

        Task<HttpStatusCode> UpdateAsync<T>(T upsertContentPageModel)
            where T : IDataModel;

        Task<HttpStatusCode> DeleteAsync(Guid documentId);
    }
}