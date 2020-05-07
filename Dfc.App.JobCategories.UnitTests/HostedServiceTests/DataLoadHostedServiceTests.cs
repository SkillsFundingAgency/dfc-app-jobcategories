using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.HostedService;
using DFC.App.JobCategories.PageService;
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
    public async Task DataLoadHostedService_StartAsync_LoadsDataIntoCosmosDb()
    {
        // arrange
        var jobCategoryRepository = A.Fake<ICosmosRepository<JobCategory>>();
        var jobProfileRepository = A.Fake<ICosmosRepository<JobProfile>>();
        var dataLoadService = A.Fake<IDataLoadService<ServiceTaxonomyApiClientOptions>>();

        var expectedCategoryResults = A.CollectionOfFake<JobCategory>(2);
        var expectedProfileResults = new List<JobProfile> {
            new JobProfile { Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"), DocumentId = Guid.NewGuid() },
            new JobProfile { Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"), DocumentId = Guid.NewGuid() },
            new JobProfile { Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"), DocumentId = Guid.NewGuid() },
            new JobProfile { Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"), DocumentId = Guid.NewGuid() },
            new JobProfile { Uri = new Uri($"http://somewhere/something/{Guid.NewGuid()}/"), DocumentId = Guid.NewGuid() },
        };

        A.CallTo(() => dataLoadService.GetAllAsync("JobProfile")).Returns(JsonConvert.SerializeObject(expectedProfileResults));
        A.CallTo(() => dataLoadService.GetAllAsync("JobCategory")).Returns(JsonConvert.SerializeObject(expectedCategoryResults));

        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).Returns(HttpStatusCode.OK);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).Returns(HttpStatusCode.OK);

        var dataLoadHostedService = new DataLoadHostedService(dataLoadService, jobProfileRepository, jobCategoryRepository);

        // act
        await dataLoadHostedService.StartAsync(CancellationToken.None).ConfigureAwait(false);

        // assert
        A.CallTo(() => jobCategoryRepository.UpsertAsync(A<JobCategory>.Ignored)).MustHaveHappened(2, Times.Exactly);
        A.CallTo(() => jobProfileRepository.UpsertAsync(A<JobProfile>.Ignored)).MustHaveHappened(5, Times.Exactly);
    }
}