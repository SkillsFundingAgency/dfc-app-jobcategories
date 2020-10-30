using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Data.Contracts
{
    public interface ICacheReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
