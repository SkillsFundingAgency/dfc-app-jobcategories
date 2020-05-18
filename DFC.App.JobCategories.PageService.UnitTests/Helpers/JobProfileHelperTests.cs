using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.App.JobCategories.PageService.Extensions;
using DFC.App.JobCategories.PageService.Helpers;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.PageService.UnitTests.Helpers
{
    [Trait("Category", "Job Profile Helper Unit Tests")]
    public class JobProfileHelperTests
    {
        [Fact]
        public async Task JobProfileHelperWhenAddOccupationAndLabelsReturnsOccupationsAndLabels()
        {
            //Arrange
            var fakeApiService = A.Fake<IApiDataService<ServiceTaxonomyApiClientOptions>>();
            var jobProfileHelper = new JobProfileHelper(new ApiExtensions(fakeApiService));

            A.CallTo(() => fakeApiService.GetByIdAsync<JobProfileApiResponse>(nameof(JobProfile).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobProfileApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<JobCategoryApiResponse>(nameof(JobCategory).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetJobCategoryApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationApiResponse());
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).Returns(TestHelpers.GetOccupationLabelApiResponse());

            //Act
            var result = await jobProfileHelper.AddOccupationAndLabels(TestHelpers.GetJobProfile()).ConfigureAwait(false);

            //Assert
            Assert.NotNull(result.Occupation);
            Assert.Equal(2, result.Occupation.OccupationLabels.Count());

            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationApiResponse>(nameof(Occupation).ToLower(), A<Guid>.Ignored)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => fakeApiService.GetByIdAsync<OccupationLabelApiResponse>(nameof(OccupationLabel).ToLower(), A<Guid>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}
