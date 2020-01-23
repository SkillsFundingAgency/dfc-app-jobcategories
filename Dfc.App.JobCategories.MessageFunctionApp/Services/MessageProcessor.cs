using Dfc.App.JobCategories.Data.Enums;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        public Task<HttpStatusCode> ProcessAsync(string message, long sequenceNumber, MessageContentType messageContentType, MessageAction messageAction)
        {
            throw new NotImplementedException();
        }
    }
}