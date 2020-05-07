using DFC.App.JobCategories.Data.Contracts;
using DFC.App.JobCategories.Data.Models;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.ContentPageServiceTests
{
    [Trait("Category", "Page Service Unit Tests")]
    public class ContentPageServiceGetByIdTests
    {
        [Fact]
        public void ContentPageServiceGetByIdReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            var expectedResult = A.Fake<JobCategory>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var result = contentPageService.GetByIdAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void ContentPageServiceGetByIdReturnsNullWhenMissingRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var repository = A.Fake<ICosmosRepository<JobCategory>>();
            JobCategory? expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).Returns(expectedResult);

            var contentPageService = new ContentPageService<JobCategory>(repository);

            // act
            var result = contentPageService.GetByIdAsync(documentId).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobCategory, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
