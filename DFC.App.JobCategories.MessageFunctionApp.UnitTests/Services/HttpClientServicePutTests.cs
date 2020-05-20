using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.MessageFunctionApp.Models;
using DFC.App.JobCategories.MessageFunctionApp.Services;
using DFC.App.JobCategories.MessageFunctionApp.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.MessageFunctionApp.UnitTests.Services
{
    [Trait("Messaging Function", "HttpClientService Put Tests")]
    public class HttpClientServicePutTests
    {
        private readonly ILogger logger;
        private readonly ContentPageClientOptions contentPageClientOptions;

        public HttpClientServicePutTests()
        {
            logger = A.Fake<ILogger>();
            contentPageClientOptions = new ContentPageClientOptions
            {
                BaseAddress = new Uri("https://somewhere.com", UriKind.Absolute),
            };
        }
    }
}
