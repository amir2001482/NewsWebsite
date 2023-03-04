using AutoMapper;
using NewsWebsite.Entities;
using NewsWebsite.Entities.identity;
using NewsWebsite.ViewModels.Category;
using NewsWebsite.ViewModels.Manage;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.RoleManager;
using NewsWebsite.ViewModels.Tag;
using NewsWebsite.ViewModels.UserManager;
using NewsWebsite.ViewModels.Video;
using System;
using System.Collections.Generic;
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
            CreateMap<News, NewsViewModel>().ReverseMap();
        }
    }
}
