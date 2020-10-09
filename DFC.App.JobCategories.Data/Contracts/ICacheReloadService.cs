using DFC.App.JobCategories.Data.Models.API;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);
        Task<IList<JobCategoriesSummaryItemModel>?> GetSummaryListAsync();
        Task ProcessSummaryListAsync(IList<JobCategoriesSummaryItemModel> summaryList, CancellationToken stoppingToken);
        Task GetAndSaveItemAsync(JobCategoriesSummaryItemModel item, CancellationToken stoppingToken);
    }
}
