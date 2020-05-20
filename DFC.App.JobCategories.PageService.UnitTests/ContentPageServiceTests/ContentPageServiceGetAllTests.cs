using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.ContentPageServiceTests
{
    [Trait("Category", "Page Service Unit Tests")]
    public class ContentPageServiceGetAllTests
    {
        [Fact]
        public async Task ContentPageServiceGetAllListReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            var expectedResults = A.CollectionOfFake<JobCategory>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var results = await contentPageService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }

        [Fact]
        public async Task ContentPageServiceGetAllListReturnsNullWhenMissingRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<JobCategory>>();
            IEnumerable<JobCategory>? expectedResults = null;

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var results = await contentPageService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }
    }
}
