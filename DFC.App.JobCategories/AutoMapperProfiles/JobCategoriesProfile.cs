using AutoMapper;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Data.Models.API;
using DFC.Content.Pkg.Netcore.Data.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class JobCategoriesProfile : Profile
    {
        public JobCategoriesProfile()
        {
            CreateMap<LinkDetails, JobProfileApiResponse>()
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.Description, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore())
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore());

            CreateMap<LinkDetails, OccupationApiResponse>()
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore())
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore());

            CreateMap<LinkDetails, OccupationLabelApiResponse>()
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore())
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore());

            CreateMap<JobCategoryApiResponse, JobCategory>()
                .ForMember(d => d.Uri, s => s.MapFrom(x => x.Url))
                .ForMember(d => d.DateModified, s => s.MapFrom(x => DateTime.UtcNow))
                .ForMember(d => d.JobProfiles, s => s.MapFrom(x => x.ContentItems));

            CreateMap<JobProfileApiResponse, JobProfile>()
                .ForMember(d => d.Uri, s => s.MapFrom(x => x.Url))
                .ForMember(d => d.DateModified, s => s.MapFrom(x => DateTime.UtcNow))
                .ForMember(d => d.Occupation, s => s.MapFrom(x => x.ContentItems[0]));

            CreateMap<OccupationApiResponse, Occupation>()
                .ForMember(d => d.Uri, s => s.MapFrom(x => x.Url))
                .ForMember(d => d.OccupationLabels, s => s.MapFrom(x => x.ContentItems));

            CreateMap<OccupationLabelApiResponse, OccupationLabel>()
                .ForMember(d => d.Uri, s => s.MapFrom(x => x.Url));
        }
    }
}
