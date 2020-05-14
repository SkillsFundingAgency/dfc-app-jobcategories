using Microsoft.Azure.ServiceBus;

namespace DFC.App.JobCategories.MessageFunctionApp.Services
{
    public interface IMessagePropertiesService
    {
        long GetSequenceNumber(Message message);
    }
}