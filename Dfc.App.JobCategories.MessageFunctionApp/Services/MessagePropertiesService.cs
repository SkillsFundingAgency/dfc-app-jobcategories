using Microsoft.Azure.ServiceBus;

namespace Dfc.App.JobCategories.MessageFunctionApp.Services
{
    public class MessagePropertiesService : IMessagePropertiesService
    {
        public long GetSequenceNumber(Message message)
        {
            return (message?.SystemProperties?.SequenceNumber).GetValueOrDefault();
        }
    }
}