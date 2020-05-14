using DFC.App.JobCategories.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService.Helpers
{
    public interface IJobProfileHelper
    {
        Task<IEnumerable<JobProfile>> AddOccupationAndLabels(IEnumerable<JobProfile> jobProfiles);

        Task<JobProfile> AddOccupationAndLabels(JobProfile jobProfile);
    }
}
