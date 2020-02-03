using Dfc.App.JobCategories.Data.Enums;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.MessageFunctionApp.Services
{
    public interface IMessageProcessor
    {
        Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageAction messageAction);
    }
}