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
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            var fakeJobProfilePageContentService = A.Fake<IContentPageService<JobProfile>>();
            var fakeJobCategoryPageContentService = A.Fake<IContentPageService<JobCategory>>();
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory?>.Ignored)).Returns(System.Net.HttpStatusCode.OK);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile?>.Ignored)).Returns(System.Net.HttpStatusCode.OK);

            var eventProcessingService = new EventProcessingService(A.Fake<ILogger<EventProcessingService>>(), fakeJobCategoryPageContentService, fakeJobProfilePageContentService, fakeApiService, jobProfileHelper);

            //Act
            var result = await eventProcessingService.AddOrUpdateAsync(new Uri($"http://somehost.com/jobcategory/{Guid.NewGuid()}")).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, result);
            A.CallTo(() => fakeJobCategoryPageContentService.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeJobProfilePageContentService.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).MustHaveHappened(2, Times.Exactly);
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).MustHaveHappened(4, Times.Exactly);
        }
    }
}
