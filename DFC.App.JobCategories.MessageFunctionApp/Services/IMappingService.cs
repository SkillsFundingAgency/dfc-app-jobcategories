using DFC.App.JobCategories.Data.Models;

namespace DFC.App.JobCategories.MessageFunctionApp.Services
{
    public interface IMappingService
    {
        JobCategory MapToContentPageModel(string message, long sequenceNumber);
    }
}