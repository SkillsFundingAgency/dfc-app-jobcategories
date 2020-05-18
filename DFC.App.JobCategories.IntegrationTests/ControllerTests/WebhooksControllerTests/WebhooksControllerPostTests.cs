using DFC.App.JobCategories.IntegrationTests.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.IntegrationTests.ControllerTests.WebhooksControllerTests
{
    [Trait("Category", "Webhooks Controller Unit Tests")]
    public class WebhooksControllerPostTests : BaseWebhooksController
    {
        public static IEnumerable<object[]> PublishedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypePublished },
            new object[] { MediaTypeNames.Application.Json, EventTypeDraft },
        };

        public static IEnumerable<object[]> DeletedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypeDeleted },
        };

        public static IEnumerable<object[]> InvalidIdValues => new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { "Not a Guid" },
        };

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsSuccess(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridDataModel { Api = new Uri($"http://somehost.com/jobprofile/{Guid.NewGuid()}"), Author = "Test Author", DisplayText = "Test Display", VersionId = "abc123" });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

            A.CallTo(() => FakeEventMessageService.AddOrUpdateAsync(A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeEventMessageService.AddOrUpdateAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(DeletedEvents))]
        public async Task WebhooksControllerDeletePostReturnsSuccess(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridDataModel { Api = new Uri($"http://somehost.com/jobprofile/{Guid.NewGuid()}"), Author = "Test Author", DisplayText = "Test Display", VersionId = "abc123" });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            var result = await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        //[Theory]
        //[MemberData(nameof(PublishedEvents))]
        //public async Task WebhooksControllerPublishUpdatePostReturnsAlreadyReported(string mediaTypeName, string eventType)
        //{
        //    // Arrange
        //    const HttpStatusCode expectedResponse = HttpStatusCode.OK;
        //    var expectedValidApiModel = BuildValidContactUsApiDataModel();
        //    var eventGridEvents = BuildValidEventGridEvent(eventType, "http://localhost");
        //    var controller = BuildWebhooksController(mediaTypeName);
        //    controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

        //    A.CallTo(() => FakeApiDataProcessorService.GetAsync<ContactUsApiDataModel>(A<Uri>.Ignored)).Returns(expectedValidApiModel);
        //    A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

        //    // Act
        //    var result = await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false);

        //    // Assert
        //    A.CallTo(() => FakeApiDataProcessorService.GetAsync<ContactUsApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
        //    A.CallTo(() => FakeEventMessageService.UpdateAsync(A<ContentPageModel>.Ignored)).MustHaveHappenedOnceExactly();
        //    A.CallTo(() => FakeEventMessageService.CreateAsync(A<ContentPageModel>.Ignored)).MustNotHaveHappened();
        //    A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
        //    var okResult = Assert.IsType<OkResult>(result);

        //    Assert.Equal((int)expectedResponse, okResult.StatusCode);

        //    controller.Dispose();
        //}

        [Theory]
        [MemberData(nameof(InvalidIdValues))]
        public async Task WebhooksControllerPostReturnsErrorForInvalidId(string id)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, "http://localhost");
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            eventGridEvents.First().Id = id;
            controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false)).ConfigureAwait(false);
            controller.Dispose();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForInvalidEventType()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent("Unknown", "http://localhost");
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false)).ConfigureAwait(false);
            controller.Dispose();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForInvalidUrl()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, "http://localhost");
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            eventGridEvents.First().Data = "http:http://badUrl";
            controller.HttpContext.Request.Body = BuildStreamFromEventGridEvents(eventGridEvents);

            A.CallTo(() => FakeEventMessageService.DeleteAsync(A<Uri>.Ignored)).Returns(HttpStatusCode.OK);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveJobCategoriesEvents().ConfigureAwait(false)).ConfigureAwait(false);
            controller.Dispose();
        }

        private static EventGridEvent[] BuildValidEventGridEvent(string eventType, object data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"/content/jobprofile/{Guid.NewGuid()}",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            return models;
        }

        private static Stream BuildStreamFromEventGridEvents(EventGridEvent[] eventGridEvents)
        {
            var jsonData = JsonConvert.SerializeObject(eventGridEvents);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }
    }
}