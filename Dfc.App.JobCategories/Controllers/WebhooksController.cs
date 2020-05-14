using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Controllers
{
    [Route("api/webhook")]
    public class WebhooksController : Controller
    {
        private const string EventTypePublished = "Published";
        private const string EventTypeDraft = "Draft";
        private const string EventTypeDeleted = "Deleted";

        private readonly ILogger<WebhooksController> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventProcessingService eventProcessingService;

        public WebhooksController(ILogger<WebhooksController> logger, AutoMapper.IMapper mapper, IEventProcessingService eventProcessingService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventProcessingService = eventProcessingService;
        }

        [HttpPost]
        [Route("ReceiveJobCategoryEvents")]
        public async Task<IActionResult> ReceiveContactUsEvents()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            string requestContent = await reader.ReadToEndAsync().ConfigureAwait(false);
            logger.LogInformation($"Received events: {requestContent}");

            var eventGridSubscriber = new EventGridSubscriber();
            eventGridSubscriber.AddOrUpdateCustomEventMapping(EventTypePublished, typeof(string));
            eventGridSubscriber.AddOrUpdateCustomEventMapping(EventTypeDraft, typeof(string));
            eventGridSubscriber.AddOrUpdateCustomEventMapping(EventTypeDeleted, typeof(string));
            var eventGridEventz = JsonConvert.DeserializeObject<EventGridEvent[]>(requestContent)
                .FirstOrDefault();
            var eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

            foreach (var eventGridEvent in eventGridEvents)
            {
                if (eventGridEvent.Data is SubscriptionValidationEventData)
                {
                    var eventData = eventGridEvent.Data as SubscriptionValidationEventData;

                    logger.LogInformation($"Got SubscriptionValidation event data, validationCode: {eventData!.ValidationCode},  validationUrl: {eventData.ValidationUrl}, topic: {eventGridEvent.Topic}");

                    // Do any additional validation (as required) such as validating that the Azure resource ID of the topic matches
                    // the expected topic and then return back the below response
                    var responseData = new SubscriptionValidationResponse()
                    {
                        ValidationResponse = eventData.ValidationCode,
                    };

                    return Ok(responseData);
                }
                else if (eventGridEvent.Data is StorageBlobCreatedEventData)
                {
                    var eventData = eventGridEvent.Data as StorageBlobCreatedEventData;
                    logger.LogInformation($"Got BlobCreated event data, blob URI {eventData!.Url}");
                }
                else
                {
                    var id = eventGridEvent.Id;
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new InvalidDataException($"Invalid Id for EventGridEvent.Id '{eventGridEvent.Id}'");
                    }

                    if (!Enum.IsDefined(typeof(MessageAction), eventGridEvent.EventType))
                    {
                        throw new InvalidDataException($"Invalid event type '{eventGridEvent.EventType}' received for Event Id: {id}, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
                    }

                    if (!Uri.TryCreate((string)eventGridEvent.Data, UriKind.Absolute, out Uri? url))
                    {
                        throw new InvalidDataException($"Invalid Url '{(string)eventGridEvent.Data}' received for Event Id: {id}");
                    }

                    var eventType = Enum.Parse<MessageAction>(eventGridEvent.EventType, true);

                    logger.LogInformation($"Got Event Id: {id}: {eventType} {url}");

                    var result = await ProcessMessageAsync(eventType, eventGridEvent.Subject, id, url).ConfigureAwait(false);

                    var statusCodeResult = new StatusCodeResult((int)result);
                    return statusCodeResult;
                }
            }

            return Ok();
        }

        private async Task<HttpStatusCode> ProcessMessageAsync(MessageAction eventType, string subject, string id, Uri url)
        {
            logger.LogInformation($"Processing message for: {id}: {eventType} {url}");

            switch (eventType)
            {
                case MessageAction.Deleted:
                    return await eventProcessingService.DeleteAsync(url).ConfigureAwait(false);
                case MessageAction.Published:
                case MessageAction.Draft:
                    return await eventProcessingService.AddOrUpdateAsync(url).ConfigureAwait(false);
                default:
                    logger.LogError($"Got unknown event type - {eventType}");
                    return HttpStatusCode.BadRequest;
            }
        }
    }
}