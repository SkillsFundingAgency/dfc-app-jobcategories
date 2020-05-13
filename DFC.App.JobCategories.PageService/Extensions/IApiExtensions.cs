using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.Extensions
{
    public interface IApiExtensions
    {
        Task<IEnumerable<T>> LoadDataAsync<T>(string contentType)
            where T : class;

        Task<T> LoadDataByIdAsync<T>(string contentType, Guid id)
            where T : class;
    }
}
