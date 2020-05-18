using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.ContentPageServiceTests
{
    [Trait("Category", "Page Service Unit Tests")]
    public class ContentPageServiceUpdateTests
    {
        [Fact]
        public void ContentPageServiceUpdateReturnsSuccessWhenContentPageReplaced()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            var contentPageModel = A.Fake<JobCategory>();
            var expectedResult = A.Fake<JobCategory>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var result = contentPageService.UpsertAsync(contentPageModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(contentPageModel)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async System.Threading.Tasks.Task ContentPageServiceUpdateReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange
            JobCategory? jobCategory = null;
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await contentPageService.UpsertAsync(jobCategory).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'contentPageModel')", exceptionResult.Message);
        }

        [Fact]
        public void ContentPageServiceUpdateReturnsNullWhenContentPageNotReplaced()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            var contentPageModel = A.Fake<JobCategory>();
            var expectedResult = A.Dummy<JobCategory>();

            A.CallTo(() => repository.UpsertAsync(contentPageModel)).Returns(HttpStatusCode.BadRequest);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var result = contentPageService.UpsertAsync(contentPageModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(contentPageModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void ContentPageServiceUpdateReturnsNullWhenMissingRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<JobCategory>>();
            var contentPageModel = A.Fake<JobCategory>();
            ContentPageModel? expectedResult = null;

            A.CallTo(() => repository.UpsertAsync(contentPageModel)).Returns(HttpStatusCode.FailedDependency);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var result = contentPageService.UpsertAsync(contentPageModel).Result;

            // assert
            A.CallTo(() => repository.UpsertAsync(contentPageModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustNotHaveHappened();
            A.Equals(result, expectedResult);
        }
    }
}
