using System.Net.Http;

namespace DFC.App.JobCategories.MessageFunctionApp.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}