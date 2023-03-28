using AutoMapper;
using NewsWebsite.Common;
using NewsWebsite.Entities;
using NewsWebsite.Entities.identity;
using NewsWebsite.ViewModels.Category;
using NewsWebsite.ViewModels.Comments;
using NewsWebsite.ViewModels.Manage;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.RoleManager;
using NewsWebsite.ViewModels.Tag;
using NewsWebsite.ViewModels.UserManager;
using NewsWebsite.ViewModels.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsWebsite.IocConfig.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap()
                .ForMember(p => p.Parent, opt => opt.Ignore())
                .ForMember(p => p.Categories, opt => opt.Ignore())
                .ForMember(p => p.NewsCategories, opt => opt.Ignore());

            CreateMap<Role, RolesViewModel>().ReverseMap()
                    .ForMember(p => p.Users, opt => opt.Ignore())
                    .ForMember(p => p.Claims, opt => opt.Ignore());

            CreateMap<Tag, TagViewModel>().ReverseMap()
                   .ForMember(p => p.NewsTags, opt => opt.Ignore());

            CreateMap<Video, VideoViewModel>().ReverseMap()
                .ForMember(p => p.PublishDateTime, opt => opt.Ignore());

            CreateMap<User, UsersViewModel>().ReverseMap()
                .ForMember(p => p.Claims, opt => opt.Ignore())
                .ForMember(p => p.Bookmarks, opt => opt.Ignore())
                .ForMember(p => p.News, opt => opt.Ignore());
            CreateMap<User, ProfileViewModel>().ReverseMap()
               .ForMember(p => p.Claims, opt => opt.Ignore())
               .ForMember(p => p.Bookmarks, opt => opt.Ignore())
               .ForMember(p => p.News, opt => opt.Ignore());
            CreateMap<News, NewsViewModel>()
                .ForMember(p => p.AuthorName, opt => opt.MapFrom(d => d.User.FirstName + " " + d.User.LastName))
                .ForMember(p => p.ShortTitle, opt => opt.MapFrom(d => d.Title.Length > 60 ? d.Title.Substring(0, 60) : d.Title))
                .ForMember(p => p.NumberOfVisit, opt => opt.MapFrom(d => d.Visits.Select(c => c.NumberOfVisit).Sum()))
                .ForMember(p => p.NumberOfDisLike, opt => opt.MapFrom(d => d.Likes.Where(d => d.IsLiked == false).Count()))
                .ForMember(p => p.NumberOfLike, opt => opt.MapFrom(d => d.Likes.Where(d => d.IsLiked == true).Count()))
                .ForMember(p => p.PersianPublishDate, opt => opt.MapFrom(d => d.PublishDateTime == null ? "-" : d.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss")))
                .ForMember(p => p.NewsType, opt => opt.MapFrom(d => d.IsInternal == true ? "داخلی" : "خارجی"))
                .ForMember(p => p.Status, opt => opt.MapFrom(d => d.IsPublish == false ? "پیش نویس" : (d.PublishDateTime > DateTime.Now ? "انتشار در آینده" : "منتشر شده")))
                .ForMember(p => p.NumberOfComment, opt => opt.MapFrom(d => d.Comments.Count()));
            CreateMap<NewsViewModel, News>();
            CreateMap<Comment, CommentViewModel>().ReverseMap();
            CreateMap<Video, VideoViewModel>()
                .ForMember(p => p.PersianPublishDateTime, opt => opt.MapFrom(d => d.PublishDateTime.ConvertMiladiToShamsi("yyyy/MM/dd ساعت hh:mm:ss")));


        }
    }
}
