using Dfc.App.JobCategories.Data.Models;
using Dfc.App.JobCategories.Repositories;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.Services
{
    public class JobCategoriesService : IJobCategoriesService
    {
        private readonly ICosmosRepository<JobCategoriesDataModel> repository;

        public JobCategoriesService(ICosmosRepository<JobCategoriesDataModel> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }
    }
}