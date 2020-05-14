using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using FakeItEasy;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            var dataLoadService = new ApiDataService<ServiceTaxonomyApiClientOptions>(httpClient, A.Fake<ServiceTaxonomyApiClientOptions>());

            // act
            var result = await dataLoadService.GetAllAsync<JobProfileApiResponse>("JobProfile").ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task DataLoadServiceGetJobProfileByIdReturnsJobProfile()
        {
            // arrange
            var apiResponse = File.ReadAllText(Directory.GetCurrentDirectory() + "/DataLoadServiceTests/Files/DataLoadService_GetById_JobProfile_Response.json");
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(apiResponse) };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://somebaseaddress") };
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var dataLoadService = new ApiDataService<ServiceTaxonomyApiClientOptions>(httpClient, A.Fake<ServiceTaxonomyApiClientOptions>());

            // act
            var result = await dataLoadService.GetByIdAsync<JobProfile>("JobProfile", Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.NotNull(result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task DataLoadServiceGetJobProfileByIdReturnsEmptyResponse()
        {
            // arrange
            var apiResponse = File.ReadAllText(Directory.GetCurrentDirectory() + "/DataLoadServiceTests/Files/DataLoadService_GetAll_JobProfile_Response.json");
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(apiResponse) };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler) { BaseAddress = new Uri("http://somebaseaddress") };
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var dataLoadService = new ApiDataService<ServiceTaxonomyApiClientOptions>(httpClient, A.Fake<ServiceTaxonomyApiClientOptions>());

            // act
            var result = await dataLoadService.GetByIdAsync<JobProfile>("JobProfile", Guid.NewGuid()).ConfigureAwait(false);

            // assert
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Null(result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}
