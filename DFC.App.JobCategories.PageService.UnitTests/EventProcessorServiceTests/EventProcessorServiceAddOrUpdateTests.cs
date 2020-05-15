using Castle.Core.Logging;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.EventProcessorServices;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
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

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).Returns(GetJobCategoryApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).Returns(GetOccupationApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(GetOccupationLabelApiResponse());

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

        private OccupationLabelApiResponse GetOccupationLabelApiResponse()
        {
            return new OccupationLabelApiResponse
            {
                Title = "An occupation label",
                Uri = new Uri("http://somehost/someresource/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9")
            };
        }

        private OccupationApiResponse GetOccupationApiResponse()
        {
            return new OccupationApiResponse
            {
                Title = "Test Occupation",
                Uri = new Uri("http://somehost/someresource/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupationlabel", new DynamicLink() { Href = new Uri("http://somehost/someresource/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9"), Relationship="ncs__hasAltLabel" }) },
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupationlabel", new DynamicLink() { Href = new Uri("http://somehost/someresource/occupationlabel/a4415817-0ca4-487e-af74-2e8276c606d9"), Relationship="ncs__hasAltLabel" }) },
                },
            };
        }

        private JobCategoryApiResponse GetJobCategoryApiResponse()
        {
            return new JobCategoryApiResponse
            {
                Description = "A Test Job Category",
                Title = "Test Job Category",
                WebsiteUri = new Uri("http://somehost/someresource/occupation/18419c07-400b-4de1-a893-36d419b18ec7/"),
                Uri = new Uri("http://somehost/someresource/occupation/18419c07-400b-4de1-a893-36d419b18ec7/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("jobprofile", new DynamicLink() { Href = new Uri($"http://somehost/someresource/jobprofile/{Guid.NewGuid()}/") }) },
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("jobprofile", new DynamicLink() { Href = new Uri($"http://somehost/someresource/jobprofile/{Guid.NewGuid()}/") }) },
                },
            };
        }

        private JobProfileApiResponse GetJobProfileApiResponse()
        {
            return new JobProfileApiResponse
            {
                Description = "A Test Job Profile",
                Title = "Test Job Profile",
                Uri = new Uri($"http://somehost/someresource/jobprofile/{Guid.NewGuid()}"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri("http://somehost/someresource/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b") }) },
                },
            };
        }
    }
}
