using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.HostedService;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using FakeItEasy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Trait("Category", "Data Load Hosted Service Unit Tests")]
public class DataLoadHostedServiceTests
{
    [Fact]
    public async Task DataLoadHostedServiceStartAsyncLoadsDataIntoCosmosDb()
    {
        // arrange
        var jobCategoryRepository = A.Fake<IContentPageService<JobCategory>>();
        var jobProfileRepository = A.Fake<IContentPageService<JobProfile>>();
        var dataLoadService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();

        var occupationGuid = Guid.NewGuid();

        var expectedCategoryResults = A.CollectionOfFake<JobCategoryApiResponse>(2);
        var expectedProfileResults = new List<JobProfileApiResponse>
        {
            new JobProfileApiResponse
            {
                Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/") }) },
                },
            },
            new JobProfileApiResponse
            {
                Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/") }) },
                },
            },
            new JobProfileApiResponse
            {
                Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/") }) },
                },
            },
            new JobProfileApiResponse
            {
                Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/") }) },
                },
            },
            new JobProfileApiResponse
            {
                Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"),
                Links = new List<Link>
                {
                    new Link { LinkValue = new KeyValuePair<string, DynamicLink>("occupation", new DynamicLink() { Href = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/") }) },
                },
            },
        };

        var expectedOccupation = new OccupationApiResponse
        {
            Title = "Some Occupation",
            Uri = new Uri($"http://somehost/someresource/occupation/{occupationGuid}/"),
            Links = new List<Link>
            {
                new Link
                {
                    LinkValue = new KeyValuePair<string, DynamicLink>("occupationlabel", new DynamicLink() { Href = new Uri($"http://somehost/someresource/somelabel1/{Guid.NewGuid()}/"), Relationship = "ncs__hasAltLabel" }),
                },
                new Link
                {
                    LinkValue = new KeyValuePair<string, DynamicLink>("occupationlabel", new DynamicLink() { Href = new Uri($"http://somehost/someresource/somelabel2/{Guid.NewGuid()}/"), Relationship = "ncs__hasAltLabel" }),
                },
            },
        };

        var expectedOccupationLabel = new OccupationLabelApiResponse() { Title = "Some label", Uri = new Uri($"http://somehost/someresource/somelabel/{Guid.NewGuid()}/") };

        A.CallTo(() => dataLoadService.GetAllAsync<JobProfileApiResponse>("JobProfile")).Returns(expectedProfileResults);
        A.CallTo(() => dataLoadService.GetAllAsync<JobCategoryApiResponse>("JobCategory")).Returns(expectedCategoryResults);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationLabelApiResponse>("OccupationLabel", A<Guid>.Ignored)).Returns(expectedOccupationLabel);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationApiResponse>("Occupation", A<Guid>.Ignored)).Returns(expectedOccupation);

        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).Returns(HttpStatusCode.OK);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).Returns(HttpStatusCode.OK);

        var dataLoadHostedService = new DataLoadHostedService(new ApiExtensions(dataLoadService), jobProfileRepository, jobCategoryRepository, new JobProfileHelper(new ApiExtensions(dataLoadService)));

        // act
        await dataLoadHostedService.StartAsync(CancellationToken.None).ConfigureAwait(false);

        // assert
        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(2, Times.Exactly);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(5, Times.Exactly);

        A.CallTo(() => dataLoadService.GetAllAsync<JobProfileApiResponse>("JobProfile")).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => dataLoadService.GetAllAsync<JobCategoryApiResponse>("JobCategory")).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationApiResponse>("Occupation", A<Guid>.Ignored)).MustHaveHappened(5, Times.Exactly);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationLabelApiResponse>("OccupationLabel", A<Guid>.Ignored)).MustHaveHappened(10, Times.Exactly);
    }
}