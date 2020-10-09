using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.Data.Models;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.PageService
{
    public class EventGridService : IEventGridService
    {
        private readonly ILogger<EventGridService> logger;
        private readonly IEventGridClientService eventGridClientService;
        private readonly EventGridPublishClientOptions eventGridPublishClientOptions;

        public EventGridService(ILogger<EventGridService> logger, IEventGridClientService eventGridClientService, EventGridPublishClientOptions eventGridPublishClientOptions)
        {
            this.logger = logger;
            this.eventGridClientService = eventGridClientService;
            this.eventGridPublishClientOptions = eventGridPublishClientOptions;
        }

        private static bool IsValidEventGridPublishClientOptions(ILogger<EventGridService> logger, EventGridPublishClientOptions? eventGridPublishClientOptions)
        {
            _ = eventGridPublishClientOptions ?? throw new ArgumentNullException(nameof(eventGridPublishClientOptions));

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicEndpoint))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicEndpoint)}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.TopicKey))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.TopicKey)}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(eventGridPublishClientOptions.SubjectPrefix))
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.SubjectPrefix)}");
                return false;
            }

            if (eventGridPublishClientOptions.ApiEndpoint == null)
            {
                logger.LogWarning($"EventGridPublishClientOptions is missing a value for: {nameof(eventGridPublishClientOptions.ApiEndpoint)}");
                return false;
            }

            return true;
        }

        public async Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, JobCategory updatedJobCategory)
        {
            _ = updatedJobCategory ?? throw new ArgumentNullException(nameof(updatedJobCategory));

            if (!IsValidEventGridPublishClientOptions(logger, eventGridPublishClientOptions))
            {
                logger.LogWarning("Unable to send to event grid due to invalid EventGridPublishClientOptions options");
                return;
            }

            var logMessage = $"{webhookCacheOperation} - {updatedJobCategory.Id} - {updatedJobCategory.CanonicalName}";
            logger.LogInformation($"Sending Event Grid message for: {logMessage}");

            var eventGridEvents = new List<EventGridEvent>
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{eventGridPublishClientOptions.SubjectPrefix}{updatedJobCategory.Id}",
                    Data = new EventGridEventData
                    {
                        ItemId = updatedJobCategory.Id.ToString(),
                        Api = $"{eventGridPublishClientOptions.ApiEndpoint}/{updatedJobCategory.Id}",
                        DisplayText = updatedJobCategory.CanonicalName,
                        VersionId = updatedJobCategory.Id.ToString(),
                        Author = eventGridPublishClientOptions.SubjectPrefix,
                    },
                    EventType = webhookCacheOperation == WebhookCacheOperation.Delete ? "deleted" : "published",
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            eventGridEvents.ForEach(f => f.Validate());

            await eventGridClientService.SendEventAsync(eventGridEvents, eventGridPublishClientOptions.TopicEndpoint, eventGridPublishClientOptions.TopicKey, logMessage).ConfigureAwait(false);
        }
    }
}
