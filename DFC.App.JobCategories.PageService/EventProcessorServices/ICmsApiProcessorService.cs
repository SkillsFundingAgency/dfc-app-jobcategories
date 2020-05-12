using System;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.EventProcessorServices
{
    public interface ICmsApiProcessorService
    {
        Task<string?> GetDataFromApiAsync(Uri url, string acceptHeader);
    }
}