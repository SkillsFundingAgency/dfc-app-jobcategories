using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.ContentPageServiceTests
{
    [Trait("Category", "Page Service Unit Tests")]
    public class ContentPageServiceGetByNameTests
    {
        private const string CanonicalName = "name1";
        private readonly ICosmosRepository<JobCategory> repository;
        private readonly IContentPageService<JobCategory> contentPageService;

        public ContentPageServiceGetByNameTests()
        {
            repository = A.Fake<ICosmosRepository<JobCategory>>();
            contentPageService = new ContentPageService<JobCategory>(repository);
        }

        [Fact]
        public async Task ContentPageServiceGetByNameReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<JobCategory>();
            A.CallTo(() => repository.GetAsync( A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await contentPageService.GetByCanonicalNameAsync(CanonicalName).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync( A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public async Task ContentPageServiceGetByNameReturnsArgumentNullExceptionWhenNullNameIsUsed()
        {
            // arrange
            string? canonicalName = null;

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await contentPageService.GetByCanonicalNameAsync(canonicalName).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'canonicalName')", exceptionResult.Message);
        }

        [Fact]
        public async Task ContentPageServiceGetByNameReturnsNullWhenMissingRepository()
        {
            // arrange
            JobCategory? expectedResult = null;

            A.CallTo(() => repository.GetAsync( A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await contentPageService.GetByCanonicalNameAsync(CanonicalName).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync( A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Null(result);
        }
    }
}