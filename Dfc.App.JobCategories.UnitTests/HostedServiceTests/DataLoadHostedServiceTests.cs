using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.HostedService;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using DFC.App.JobCategories.PageService.UnitTests.Helpers;
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

        var expectedCategoryResults = new List<JobCategoryApiResponse>
        {
            TestHelpers.GetJobCategoryApiResponse(),
            TestHelpers.GetJobCategoryApiResponse(),
        };

        var expectedProfileResults = new List<JobProfileApiResponse>
        {
            TestHelpers.GetJobProfileApiResponse(),
            TestHelpers.GetJobProfileApiResponse(),
            TestHelpers.GetJobProfileApiResponse(),
            TestHelpers.GetJobProfileApiResponse(),
            TestHelpers.GetJobProfileApiResponse(),
        };

        var expectedOccupation = TestHelpers.GetOccupationApiResponse();

        var expectedOccupationLabel = TestHelpers.GetOccupationLabelApiResponse();

        A.CallTo(() => dataLoadService.GetAllAsync<JobProfileApiResponse>("jobprofile")).Returns(expectedProfileResults);
        A.CallTo(() => dataLoadService.GetAllAsync<JobCategoryApiResponse>("jobcategory")).Returns(expectedCategoryResults);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationLabelApiResponse>("occupationlabel", A<Guid>.Ignored)).Returns(expectedOccupationLabel);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationApiResponse>("occupation", A<Guid>.Ignored)).Returns(expectedOccupation);

        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).Returns(HttpStatusCode.OK);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).Returns(HttpStatusCode.OK);

        var dataLoadHostedService = new DataLoadHostedService(new ApiExtensions(dataLoadService), jobProfileRepository, jobCategoryRepository, new JobProfileHelper(new ApiExtensions(dataLoadService)));

        // act
        await dataLoadHostedService.StartAsync(CancellationToken.None).ConfigureAwait(false);

        // assert
        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(2, Times.Exactly);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(5, Times.Exactly);

        A.CallTo(() => dataLoadService.GetAllAsync<JobProfileApiResponse>("jobprofile")).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => dataLoadService.GetAllAsync<JobCategoryApiResponse>("jobcategory")).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationApiResponse>("occupation", A<Guid>.Ignored)).MustHaveHappened(5, Times.Exactly);
        A.CallTo(() => dataLoadService.GetByIdAsync<OccupationLabelApiResponse>("occupationlabel", A<Guid>.Ignored)).MustHaveHappened(10, Times.Exactly);
    }
}