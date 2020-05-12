using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.Data.UnitTests.ExtensionTests
{
    [Trait("Data Extensions", "Data Extension Tests")]
    public class ModelExtensionTests
    {
        [Fact]
        public void MapJobProfileApiResponseToJobProfileReturnsJobProfile()
        {
            // arrange
            var apiJobProfile = new JobProfileApiResponse { Description = "A Description", Title = "Comedian", Uri = new Uri($"http://someuri/jobprofile/identifier/{Guid.NewGuid()}"), Links = new List<Link> { new Link { LinkValue = new KeyValuePair<string, Models.DynamicLink>("daytodaytask", new Models.DynamicLink { Href = new Uri("http://someuri/daytotasktask/identifier") }) } } };

            // act
            var jobProfile = apiJobProfile.Map();

            // assert
            Assert.NotNull(jobProfile);
            Assert.Equal(apiJobProfile.Title, jobProfile.Title);
            Assert.Equal(apiJobProfile.Description, jobProfile.Description);
            Assert.Equal(apiJobProfile.Uri, jobProfile.Uri);
            Assert.Equal(apiJobProfile.Links, jobProfile.Links);
        }

        [Fact]
        public void MapNullJobProfileApiResponseToJobProfileThrowsInvalidOperationException()
        {
            // arrange
            JobProfileApiResponse? jp = null;

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => jp.Map());
        }

        [Fact]
        public void MapJobCategoryApiResponseToJobCategoryReturnsJobProfile()
        {
            // arrange
            var apiJobCategory = new JobCategoryApiResponse { Description = "An emergency services category", WebsiteUri = new Uri("http://somewhere/something/something-else/"), Title = "Emergency Services", Uri = new Uri($"http://someuri/jobcategory/identifier/{Guid.NewGuid()}"), Links = new List<Link> { new Link { LinkValue = new KeyValuePair<string, Models.DynamicLink>("jobprofile", new Models.DynamicLink { Href = new Uri($"http://someuri/jobprofile/identifier/{Guid.NewGuid()}") }) } } };

            // act
            var jobCategory = apiJobCategory.Map();

            // assert
            Assert.NotNull(apiJobCategory);
            Assert.Equal(apiJobCategory.Title, jobCategory.Title);
            Assert.Equal(apiJobCategory.Description, jobCategory.Description);
            Assert.Equal(apiJobCategory.Uri, jobCategory.Uri);
            Assert.Equal(apiJobCategory.Links, jobCategory.Links);
            Assert.Equal(apiJobCategory.WebsiteUri.Segments.Last().TrimEnd('/'), jobCategory.CanonicalName);
        }

        [Fact]
        public void MapNullJobCategoryApiResponseToJobCategoryThrowsInvalidOperationException()
        {
            // arrange
            JobCategoryApiResponse? jc = null;

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => jc.Map());
        }
    }
}
