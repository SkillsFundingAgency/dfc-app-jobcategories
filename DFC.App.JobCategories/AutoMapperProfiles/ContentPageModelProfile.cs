using AutoMapper;
using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.ViewModels;
using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobCategories.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class ContentPageModelProfile : Profile
    {
        public ContentPageModelProfile()
        {
            CreateMap<ContentPageModel, BodyViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Content)))
                ;

            CreateMap<JobCategory, BodyViewModel>()
                .ForMember(d => d.Content, s => s.MapFrom(a => new HtmlString(a.Description)))
                ;

            CreateMap<JobCategory, DocumentViewModel>()
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.BreadcrumbTitle, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.AlternativeNames, s => s.Ignore())
                .ForMember(d => d.Title, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Description))
                .ForMember(d => d.Keywords, s => s.Ignore())
                .ForMember(d => d.BodyViewModel, s => s.Ignore())
                ;

            CreateMap<JobCategory, HtmlHeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.MapFrom(a => a.CanonicalName))
                .ForMember(d => d.Title, s => s.MapFrom(a => a.Title))
                .ForMember(d => d.Description, s => s.MapFrom(a => a.Description))
                .ForMember(d => d.Keywords, s => s.Ignore())
                ;

            CreateMap<JobCategory, IndexDocumentViewModel>();
        }
    }
}
