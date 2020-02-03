using System.Threading.Tasks;

namespace Dfc.App.JobCategories.Services
{
    public interface IJobCategoriesService
    {
        Task<bool> PingAsync();
    }
}