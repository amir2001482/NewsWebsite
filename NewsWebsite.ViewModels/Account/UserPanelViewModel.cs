using NewsWebsite.Entities.identity;
using NewsWebsite.ViewModels.Manage;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.UserManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Account
{
    public class UserPanelViewModel
    {
        public UserPanelViewModel(ProfileViewModel user, List<NewsViewModel> bookmarks)
        {
            User = user;
            Bookmarks = bookmarks;
        }
        public ProfileViewModel User { get; set; }
        public List<NewsViewModel> Bookmarks { get; set; }
    }
}
