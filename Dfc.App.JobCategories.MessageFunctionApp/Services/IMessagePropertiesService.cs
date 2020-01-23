using Microsoft.Azure.ServiceBus;

namespace Dfc.App.JobCategories.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}