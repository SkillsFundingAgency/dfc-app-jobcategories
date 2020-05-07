using DFC.App.JobCategories.Data.Enums;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.MessageFunctionApp.Services;
using FakeItEasy;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.MessageFunctionApp.UnitTests.Services
{
    [Trait("Messaging Function", "Message Processor Tests")]
    public class MessageProcessorTests
    {
        private readonly IHttpClientService httpClientService;
        private readonly IMappingService mappingService;
        private readonly IMessageProcessor messageProcessor;

        public MessageProcessorTests()
        {
            httpClientService = A.Fake<IHttpClientService>();
            mappingService = A.Fake<IMappingService>();

            messageProcessor = new MessageProcessor(httpClientService, mappingService);
        }

        [Fact]
        public async Task ProcessAsyncWithBadMessageContentTypeReturnsException()
        {
            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await messageProcessor.ProcessAsync(string.Empty, 1, (MessageContentType)(-1), MessageAction.Published).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Unexpected content type '-1' (Parameter 'messageContentType')", exceptionResult.Message);
        }
    }
}
