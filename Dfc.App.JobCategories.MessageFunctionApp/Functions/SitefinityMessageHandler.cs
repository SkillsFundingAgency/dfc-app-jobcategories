using Dfc.App.JobCategories.Data.Enums;
using Dfc.App.JobCategories.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private readonly string classFullName = typeof(SitefinityMessageHandler).FullName;
        private readonly IMessageProcessor messageProcessor;
        private readonly IMessagePropertiesService messagePropertiesService;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public SitefinityMessageHandler(
            IMessageProcessor messageProcessor,
            IMessagePropertiesService messagePropertiesService,
            ILogService logService,
            ICorrelationIdProvider correlationIdProvider)
        {
            this.messageProcessor = messageProcessor;
            this.messagePropertiesService = messagePropertiesService;
            this.logService = logService;
            this.correlationIdProvider = correlationIdProvider;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage)
        {
            if (sitefinityMessage == null)
            {
                throw new ArgumentNullException(nameof(sitefinityMessage));
            }

            correlationIdProvider.CorrelationId = sitefinityMessage.CorrelationId;

            sitefinityMessage.UserProperties.TryGetValue("ActionType", out var actionType);
            sitefinityMessage.UserProperties.TryGetValue("CType", out var contentType);
            sitefinityMessage.UserProperties.TryGetValue("Id", out var messageContentId);

            logService.LogInformation($"{nameof(SitefinityMessageHandler)}: Received message action '{actionType}' for type '{contentType}' with Id: '{messageContentId}'");

            var message = Encoding.UTF8.GetString(sitefinityMessage?.Body);

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(sitefinityMessage));
            }

            if (!Enum.IsDefined(typeof(MessageAction), actionType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(sitefinityMessage), $"Invalid message action '{actionType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }

            if (!Enum.IsDefined(typeof(MessageContentType), contentType?.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(sitefinityMessage), $"Invalid message content type '{contentType}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            var messageAction = Enum.Parse<MessageAction>(actionType?.ToString());
            var messageContentType = Enum.Parse<MessageContentType>(contentType?.ToString());
            var sequenceNumber = messagePropertiesService.GetSequenceNumber(sitefinityMessage);

            var result = await messageProcessor.ProcessAsync(message, sequenceNumber, messageContentType, messageAction).ConfigureAwait(false);

            switch (result)
            {
                case HttpStatusCode.OK:
                    logService.LogInformation($"{classFullName}: JobProfile Id: {messageContentId}: Updated category");
                    break;

                case HttpStatusCode.Created:
                    logService.LogInformation($"{classFullName}: JobProfile Id: {messageContentId}: Created category");
                    break;

                case HttpStatusCode.AlreadyReported:
                    logService.LogInformation($"{classFullName}: JobProfile Id: {messageContentId}: Category previously updated");
                    break;

                default:
                    logService.LogWarning($"{classFullName}: JobProfile Id: {messageContentId}: category not Posted: Status: {result}");
                    break;
            }
        }
    }
}