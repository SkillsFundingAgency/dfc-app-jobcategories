using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.MessageFunctionApp.UnitTests.FakeHttpHandlers;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.DataLoadServiceTests
{
    [Trait("Category", "Data Load Service Unit Tests")]
    public class DataLoadServiceTests
    {
        
        [Fact]
        public async Task DataLoadServiceGetAllJobProfilesReturnsJobProfiles()
        {
            // arrange
            var apiResponse = File.ReadAllText(Directory.GetCurrentDirectory() + "/DataLoadServiceTests/Files/DataLoadService_GetAll_JobProfile_Response.json");
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(apiResponse) };
           
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://somebaseaddress") };
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var dataLoadService = new DataLoadService<ServiceTaxonomyApiClientOptions>(httpClient, A.Fake<ServiceTaxonomyApiClientOptions>());

            // act
            var result = await dataLoadService.GetAllAsync("JobProfile").ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, apiResponse);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}
