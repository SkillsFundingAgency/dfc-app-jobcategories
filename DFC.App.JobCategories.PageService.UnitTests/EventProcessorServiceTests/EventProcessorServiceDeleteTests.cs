using Castle.Core.Logging;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using DFC.App.JobCategories.PageService.UnitTests.Helpers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
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
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeJobCategoryPageContentService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeJobCategoryPageContentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobProfileReturnsOk()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategoryApiResponse).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeJobProfilePageContentService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeJobCategoryPageContentService.GetByQueryAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(new List<JobCategory> { TestHelpers.GetJobCategory() });
            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobprofile/46a884da-22bb-4ebe-87ac-228f42698ee2")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobProfilePageContentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobCategoryPageContentService.GetByQueryAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationReturnsOk()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile?>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).Returns(new List<JobProfile> { TestHelpers.GetJobProfile() });

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationLabelReturnsOk()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile?>.Ignored)).Returns(HttpStatusCode.OK);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).Returns(new List<JobProfile> { TestHelpers.GetJobProfile() });

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobCategoryReturnsNotFound()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeJobCategoryPageContentService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeJobCategoryPageContentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteJobProfileReturnsNotFound()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategoryApiResponse).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeJobProfilePageContentService.DeleteAsync(A<Guid>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeJobCategoryPageContentService.GetByQueryAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(new List<JobCategory> { TestHelpers.GetJobCategory() });
            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.NotFound);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/jobprofile/46a884da-22bb-4ebe-87ac-228f42698ee2")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobProfilePageContentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobCategoryPageContentService.GetByQueryAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationReturnsNotFound()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile?>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).Returns(new List<JobProfile> { TestHelpers.GetJobProfile() });

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task EventProcessingServiceDeleteOccupationLabelReturnsNotFound()
        {
            //Arrange
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile?>.Ignored)).Returns(HttpStatusCode.NotFound);
            A.CallTo(() => fakeJobProfilePageContentService.GetByQueryAsync(A<Expression<Func<JobProfile, bool>>>.Ignored)).Returns(new List<JobProfile> { TestHelpers.GetJobProfile() });

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.DeleteAsync(new Uri($"http://somehost.com/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, result);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(1, Times.Exactly);
        }

    }
}
