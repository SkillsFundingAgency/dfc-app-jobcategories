using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using DFC.App.JobCategories.PageService.UnitTests.Helpers;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using DFC.App.JobCategories.Data.Contracts;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.EventProcessorServiceTests
{

    [Trait("Category", "Event Processor Service Unit Tests")]
    public class EventProcessorServiceAddOrUpdateTests
    {
        [Fact]
        public async Task EventProcessingServiceAddOrUpdateJobCategoryReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();
            var fakeEventGridService = A.Fake<IEventGridService>();

            A.CallTo(() => fakeApiService.GetItemAsync<JobCategoryApiResponse>(A<Uri>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService, fakeEventGridService);

            //Act
            var result = await eventProcessingService.AddOrUpdateAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeApiService.GetItemAsync<JobCategoryApiResponse>(A<Uri>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceAddOrUpdateJobProfileReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();
            var fakeEventGridService = A.Fake<IEventGridService>();

            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService, fakeEventGridService);

            //Act
            var result = await eventProcessingService.AddOrUpdateAsync(new Uri($"http://somehost.com/jobprofile/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventProcessingServiceAddOrUpdateOccupationReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();
            var fakeEventGridService = A.Fake<IEventGridService>();

            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService, fakeEventGridService);

            //Act
            var result = await eventProcessingService.AddOrUpdateAsync(new Uri("http://somehost.com/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EventProcessingServiceAddOrUpdateOccupationLabelReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();
            var fakeEventGridService = A.Fake<IEventGridService>();

            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService, fakeEventGridService);

            //Act
            var result = await eventProcessingService.AddOrUpdateAsync(new Uri("http://somehost.com/occupationlabel/7032300f-bf9d-4b65-b4b5-604979573216")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeApiService.GetItemAsync<JobProfileApiResponse>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappenedOnceExactly();
        }
    }
}
