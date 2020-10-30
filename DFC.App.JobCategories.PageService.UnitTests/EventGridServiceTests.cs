using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests
{
    [Trait("Category", "Event Grid Service Tests")]
    public class EventGridServiceTests
    {
        [Fact]
        public async Task EventGridServiceSendEventAsyncInvokesEventGridClientService()
        {
            //Arrange
            var eventGridPublishClientOptions = new EventGridPublishClientOptions
            {
                TopicEndpoint = "Something",
                TopicKey = "Something",
                ApiEndpoint = new Uri("http://something.com"),
                SubjectPrefix = "Something",
            };

            var fakeEventGridClientService = A.Fake<IEventGridClientService>();

            var eventGridService = new EventGridService(A.Fake<ILogger<EventGridService>>(), fakeEventGridClientService, eventGridPublishClientOptions);

            //Act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, new JobCategory()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncDoesNotInvokeEventGridClientServiceIfMissingTopicEndpoint()
        {
            //Arrange
            var eventGridPublishClientOptions = new EventGridPublishClientOptions
            {
                TopicEndpoint = null,
                TopicKey = "Something",
                ApiEndpoint = new Uri("http://something.com"),
                SubjectPrefix = "Something",
            };

            var fakeEventGridClientService = A.Fake<IEventGridClientService>();

            var eventGridService = new EventGridService(A.Fake<ILogger<EventGridService>>(), fakeEventGridClientService, eventGridPublishClientOptions);

            //Act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, new JobCategory()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncDoesNotInvokeEventGridClientServiceIfMissingTopicKey()
        {
            //Arrange
            var eventGridPublishClientOptions = new EventGridPublishClientOptions
            {
                TopicEndpoint = "Something",
                TopicKey = null,
                ApiEndpoint = new Uri("http://something.com"),
                SubjectPrefix = "Something",
            };

            var fakeEventGridClientService = A.Fake<IEventGridClientService>();

            var eventGridService = new EventGridService(A.Fake<ILogger<EventGridService>>(), fakeEventGridClientService, eventGridPublishClientOptions);

            //Act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, new JobCategory()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncDoesNotInvokeEventGridClientServiceIfMissingApiEndpoint()
        {
            //Arrange
            var eventGridPublishClientOptions = new EventGridPublishClientOptions
            {
                TopicEndpoint = "Something",
                TopicKey = "Something",
                ApiEndpoint = null,
                SubjectPrefix = "Something",
            };

            var fakeEventGridClientService = A.Fake<IEventGridClientService>();

            var eventGridService = new EventGridService(A.Fake<ILogger<EventGridService>>(), fakeEventGridClientService, eventGridPublishClientOptions);

            //Act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, new JobCategory()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task EventGridServiceSendEventAsyncDoesNotInvokeEventGridClientServiceIfMissingSubjectPrefix()
        {
            //Arrange
            var eventGridPublishClientOptions = new EventGridPublishClientOptions
            {
                TopicEndpoint = "Something",
                TopicKey = "Something",
                ApiEndpoint = new Uri("http://something.com"),
                SubjectPrefix = null,
            };

            var fakeEventGridClientService = A.Fake<IEventGridClientService>();

            var eventGridService = new EventGridService(A.Fake<ILogger<EventGridService>>(), fakeEventGridClientService, eventGridPublishClientOptions);

            //Act
            await eventGridService.SendEventAsync(WebhookCacheOperation.CreateOrUpdate, new JobCategory()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeEventGridClientService.SendEventAsync(A<List<EventGridEvent>>.Ignored, A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
