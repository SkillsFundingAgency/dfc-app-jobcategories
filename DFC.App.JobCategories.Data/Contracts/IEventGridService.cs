using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Data.Contracts
{
    public interface IEventGridService
    {
        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, JobCategory updatedJobCategory);
    }
}
