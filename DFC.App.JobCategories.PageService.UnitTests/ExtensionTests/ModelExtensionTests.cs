using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DFC.App.JobCategories.Data.UnitTests.ExtensionTests
{
    [Trait("Data Extensions", "Data Extension Tests")]
    public class ModelExtensionTests
    {
        [Fact]
        public void MapJobCategoryApiResponseToJobCategoryReturnsJobCategory()
        {
            // arrange
            var apiOccupationLabel = new OccupationLabelApiResponse
            {
                ContentType = "OccupationLabel",
                Title = "Occupation Label",
                Url = new Uri($"http://someuri/occupationlabel/identifier/{Guid.NewGuid()}"),
            };

            var apiOccupation = new OccupationApiResponse
            {
                ContentType = "occupation",
                Title = "Occupation",
                Url = new Uri($"http://someuri/occupation/identifier/{Guid.NewGuid()}"),
                ContentItems = new List<IBaseContentItemModel>
                {
                    apiOccupationLabel,
                },
            };

            var apiJobProfile = new JobProfileApiResponse
            {
                ContentType = "JobProfile",
                Description = "A Description",
                Title = "Comedian",
                Url = new Uri($"http://someuri/jobprofile/identifier/{Guid.NewGuid()}"),
                ContentItems = new List<IBaseContentItemModel>
                {
                    apiOccupation,
                },
            };

            var apiJobCategoryId = Guid.NewGuid();

            var apiJobCategory = new JobCategoryApiResponse
            {
                Description = "An emergency services category",
                WebsiteUri = new Uri("http://somewhere/something/something-else/"),
                Title = "Emergency Services",
                Url = new Uri($"http://someuri/jobcategory/identifier/{apiJobCategoryId}"),
                CanonicalName = "emergency-services",
                ContentItems = new List<IBaseContentItemModel>
                {
                    apiJobProfile,
                },
            };

            // act
            var jobCategory = apiJobCategory.Map();

            // assert
            Assert.NotNull(apiJobCategory);
            Assert.Equal(apiJobCategoryId, jobCategory.Id);
            Assert.Equal(apiJobCategory.Title, jobCategory.Title);
            Assert.Equal(apiJobCategory.Description, jobCategory.Description);
            Assert.Equal(apiJobCategory.Url, jobCategory.Uri);
            Assert.Equal(apiJobCategory.CanonicalName, jobCategory.CanonicalName);
            Assert.NotEmpty(jobCategory.JobProfiles);
            Assert.Equal(jobCategory.JobProfiles.Single().Title, apiJobProfile.Title);
            Assert.Equal(jobCategory.JobProfiles.Single().Description, apiJobProfile.Description);
            Assert.Equal(jobCategory.JobProfiles.Single().Uri, apiJobProfile.Url);
            Assert.NotNull(jobCategory.JobProfiles.Single().Occupation);
            Assert.Equal(jobCategory.JobProfiles.Single().Occupation.Title, apiOccupation.Title);
            Assert.Equal(jobCategory.JobProfiles.Single().Occupation.Uri, apiOccupation.Url);
            Assert.NotEmpty(jobCategory.JobProfiles.Single().Occupation.OccupationLabels);
            Assert.Equal(jobCategory.JobProfiles.Single().Occupation.OccupationLabels.Single().Title, apiOccupationLabel.Title);
            Assert.Equal(jobCategory.JobProfiles.Single().Occupation.OccupationLabels.Single().Uri, apiOccupationLabel.Url);
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

        [Fact]
        public void MapNullJobProfileThrowsInvalidOperationException()
        {
            // arrange
            JobProfileApiResponse apiJobProfile = null;

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => apiJobProfile.Map());
        }
    }
}
