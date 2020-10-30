using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests
{
    [Trait("Category", "Cache Reload Service Unit Tests")]
    public class CacheReloadServiceTests
    {
        [Fact]
        public async Task CacheReloadServiceReloadConfiguresContentTypeMappingsAndReloadsInitialData()
        {
            //Arrange
            var fakeContentTypeMappingService = A.Fake<IContentTypeMappingService>();
            var fakeDocumentService = A.Fake<IDocumentService<JobCategory>>();
            var fakeCmsApiService = A.Fake<ICmsApiService>();

            A.CallTo(() => fakeDocumentService.GetAllAsync(null)).Returns(new List<JobCategory>()
            {
                new JobCategory(),
                new JobCategory(),
                new JobCategory(),
            });

            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<JobCategoriesSummaryItemModel>())
                .Returns(new List<JobCategoriesSummaryItemModel>
                {
                    new JobCategoriesSummaryItemModel(),
                    new JobCategoriesSummaryItemModel(),
                    new JobCategoriesSummaryItemModel(),
                    new JobCategoriesSummaryItemModel(),
                    new JobCategoriesSummaryItemModel(),
                });

            var cacheReloadService = new CacheReloadService(A.Fake<ILogger<CacheReloadService>>(), fakeContentTypeMappingService, fakeDocumentService, fakeCmsApiService);

            //Act
            await cacheReloadService.Reload(CancellationToken.None).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeContentTypeMappingService.AddMapping(A<string>.Ignored, A<Type>.Ignored))
                .MustHaveHappened(4, Times.Exactly);
            A.CallTo(() => fakeContentTypeMappingService.AddIgnoreRelationship(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.GetAllAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDocumentService.DeleteAsync(A<Guid>.Ignored))
                .MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => fakeCmsApiService.GetSummaryAsync<JobCategoriesSummaryItemModel>())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeCmsApiService.GetItemAsync<JobCategoryApiResponse>(A<Uri>.Ignored))
                .MustHaveHappened(5, Times.Exactly);
            A.CallTo(() => fakeDocumentService.UpsertAsync(A<JobCategory>.Ignored))
                .MustHaveHappened(5, Times.Exactly);
        }
    }
}
