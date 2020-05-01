using DFC.App.JobCategories.Data.Models;

namespace DFC.App.JobCategories.MessageFunctionApp.Services
{
    public interface IMappingService
    {
        ContentPageModel MapToContentPageModel(string message, long sequenceNumber);
    }
}