using DFC.App.JobCategories.Data.Extensions;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.PageService.UnitTests.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class TestHelpers
    {
        public static JobCategoryApiResponse GetJobCategoryApiResponse()
        {
            return new JobCategoryApiResponse
            {
                ContentType = "JobCategory",
                Description = "A Test Job Category",
                Title = "Test Job Category",
                WebsiteUri = new Uri("http://somehost/someresource/occupation/18419c07-400b-4de1-a893-36d419b18ec7/"),
                Url = new Uri("http://somehost/someresource/occupation/18419c07-400b-4de1-a893-36d419b18ec7/"),
                ContentItems = new List<IBaseContentItemModel>
                {
                    new JobProfileApiResponse()
                    {
                        ContentType = "JobProfile",
                        Title = "Job Profile",
                        Description = "Job Profile",
                        Url = new Uri($"http://somehost/someresource/jobprofile/{Guid.NewGuid()}/"),
                        ContentItems = new List<IBaseContentItemModel>()
                        {
                            new OccupationApiResponse()
                            {
                                ContentType = "occupation",
                                Title = "Test Occupation",
                                Url = new Uri("http://somehost/someresource/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b"),
                                ContentItems = new List<IBaseContentItemModel>()
                                {
                                    new OccupationLabelApiResponse()
                                    {
                                        ContentType = "OccupationLabel",
                                        Title = "Test Occupation Label",
                                        Url = new Uri("http://somehost/someresource/occupationlabel/7032300f-bf9d-4b65-b4b5-604979573216"),
                                    },
                                },
                            },
                        },
                    },
                    new JobProfileApiResponse()
                    {
                        ContentType = "JobProfile",
                        Title = "Job Profile 2",
                        Description = "Job Profile 2",
                        Url = new Uri($"http://somehost/someresource/jobprofile/{Guid.NewGuid()}/"),
                        ContentItems = new List<IBaseContentItemModel>()
                        {
                            new OccupationApiResponse()
                            {
                                ContentType = "occupation",
                                Title = "Test Occupatio 2n",
                                Url = new Uri("http://somehost/someresource/occupation/54288fad-2f99-43cb-8df0-d10d29977a4c"),
                                ContentItems = new List<IBaseContentItemModel>()
                                {
                                    new OccupationLabelApiResponse()
                                    {
                                        ContentType = "OccupationLabel",
                                        Title = "Test Occupation Label 2",
                                        Url = new Uri("http://somehost/someresource/occupationlabel/54288fad-2f99-43cb-8df0-d10d29977a4c"),
                                    },
                                },
                            },
                        },
                    },
                },
            };
        }

        public static JobProfileApiResponse GetJobProfileApiResponse()
        {
            return new JobProfileApiResponse
            {
                ContentType = "JobProfile",
                Description = "A Test Job Profile",
                Title = "Test Job Profile",
                Url = new Uri($"http://somehost/someresource/jobprofile/46a884da-22bb-4ebe-87ac-228f42698ee2"),
                ContentItems = new List<IBaseContentItemModel>()
                {
                    new OccupationApiResponse()
                    {
                        ContentType = "occupation",
                        Title = "Test Occupation",
                        Url = new Uri("http://somehost/someresource/occupation/54288fad-2f99-43cb-8df0-d10d29977a4b"),
                        ContentItems = new List<IBaseContentItemModel>()
                        {
                            new OccupationLabelApiResponse()
                            {
                                ContentType = "OccupationLabel",
                                Title = "Test Occupation Label",
                                Url = new Uri($"http://somehost/someresource/occupationlabel/7032300f-bf9d-4b65-b4b5-604979573216"),
                            },
                        },
                    },
                },
            };
        }

        public static JobCategory GetJobCategory()
        {
            return GetJobCategoryApiResponse().Map();
        }

        public static List<JobCategory> GetJobCategoryList()
        {
            return new List<JobCategory>() { GetJobCategory() };
        }
    }
}
