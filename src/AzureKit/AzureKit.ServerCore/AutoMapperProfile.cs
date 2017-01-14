using System;

namespace AzureKit
{
    public  class AutoMapperProfile : AutoMapper.Profile
    {

        protected override void Configure()
        {
            //map from UI models to docdb content models
            this.CreateMap<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>()
                .ForMember(dest => dest.HtmlContent, opt => opt.MapFrom(src => src.Content));

            //banner content
            this.CreateMap<Models.BannerContent, Data.DocDb.Models.BannerContentDocument>()
                .IncludeBase<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>();

            //simple content
            this.CreateMap<Models.SimpleContent, Data.DocDb.Models.SimpleContentDocument>()
                .IncludeBase<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>();
                

            //list contents
            this.CreateMap<Models.ListItemContent, Data.DocDb.Models.ListDetailContentDocument>()
                .IncludeBase<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>();
            this.CreateMap<Models.ListLandingContent, Data.DocDb.Models.ListLandingContentDocument>()
                .IncludeBase<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>();

            //media contents
            this.CreateMap<Models.MediaItemModel, Data.DocDb.Models.MediaItem>();
            this.CreateMap<Models.MediaGalleryContent, Data.DocDb.Models.MediaGalleryContentDocument>()
                .IncludeBase<Models.ContentModelBase, Data.DocDb.Models.SiteContentItemDocument>();

            //map from docdb content models to ui models
            this.CreateMap<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.HtmlContent))
                .ForMember(dest => dest.Html, opt => opt.MapFrom(src => new System.Web.Mvc.MvcHtmlString(src.HtmlContent)));

            //banner content
            this.CreateMap<Data.DocDb.Models.BannerContentDocument, Models.BannerContent>()
                .IncludeBase<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>();

            //simple content
            this.CreateMap<Data.DocDb.Models.SimpleContentDocument, Models.SimpleContent>()
                .IncludeBase<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>();
                

            //list content
            this.CreateMap<Data.DocDb.Models.ListDetailContentDocument, Models.ListItemContent>()
                .IncludeBase<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>();
            this.CreateMap<Data.DocDb.Models.ListLandingContentDocument, Models.ListLandingContent>()
                .IncludeBase<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>();


            //media content
            this.CreateMap<Data.DocDb.Models.MediaItem, Models.MediaItemModel>();
                
 
            this.CreateMap<Data.DocDb.Models.MediaGalleryContentDocument, Models.MediaGalleryContent>()
                .IncludeBase<Data.DocDb.Models.SiteContentItemDocument, Models.ContentModelBase>()
                .ForMember(dest => dest.BaseUrl, opt => opt.Ignore());


            //map from ui sitemap models to docdb sitemap models
            this.CreateMap<Models.SiteMap, Data.DocDb.Models.SiteMapDocument>();
            this.CreateMap<Models.SiteMapEntry, Data.DocDb.Models.SiteMapEntryDescription>();

            //map from docdb sitemap model to ui site map model
            this.CreateMap<Data.DocDb.Models.SiteMapDocument, Models.SiteMap>();
            this.CreateMap<Data.DocDb.Models.SiteMapEntryDescription, Models.SiteMapEntry>();

            //map API media details to content model
            this.CreateMap<Models.MediaUploadDetails, Models.MediaItemModel>()
                .ForMember(dest=> dest.Id, opt=>opt.UseValue(Guid.NewGuid().ToString()));
        }
    }
}