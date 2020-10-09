using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using DFC.App.JobCategories.PageService.UnitTests.Helpers;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.EventProcessorServiceTests
{

    [Trait("Category", "Event Processor Service Unit Tests")]
    public class EventProcessorServiceDeleteTests
    {
        [Fact]
        public async Task EventProcessingServiceDeleteJobCategoryReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeApiService.GetItemAsync<JobCategoryApiResponse>(A<Uri>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobProfileReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobprofile/46a884da-22bb-4ebe-87ac-228f42698ee2")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationLabelReturnsOk()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(TestHelpers.GetJobCategoryList());
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobCategoryReturnsNotFound()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(false);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobProfileReturnsNotFound()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns((List<JobCategory>?)null);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobprofile/46a884da-22bb-4ebe-87ac-228f42698ee2")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationReturnsNotFound()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns((List<JobCategory>?)null);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationLabelReturnsNotFound()
        {
            //Arrange
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns((List<JobCategory>?)null);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeDocumentService, fakeApiService);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeDocumentService.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }
    }
}
