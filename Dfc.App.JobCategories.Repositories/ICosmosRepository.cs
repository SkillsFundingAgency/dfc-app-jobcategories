using Dfc.App.JobCategories.Data.Models;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.Repositories
{
    public interface ICosmosRepository<T>
        where T : IDataModel
    {
        Task<bool> PingAsync();
    }
}