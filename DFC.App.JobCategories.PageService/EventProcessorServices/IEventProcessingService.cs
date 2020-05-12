using DFC.App.JobCategories.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.EventProcessorServices
{
    public interface IEventProcessingService
    {
        Task<HttpStatusCode> DeleteAsync(Uri url);

        Task<HttpStatusCode> AddOrUpdateAsync(Uri url);
    }
}