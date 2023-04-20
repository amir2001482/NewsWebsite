﻿using NewsWebsite.Entities.identity;
using NewsWebsite.ViewModels.News;
using NewsWebsite.ViewModels.UserManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Account
{
    public class UserPanelViewModel
    {
        public UserPanelViewModel(UsersViewModel user, List<NewsViewModel> bookmarks)
        {
            User = user;
            Bookmarks = bookmarks;
        }
        public UsersViewModel User { get; set; }
        public List<NewsViewModel> Bookmarks { get; set; }
    }
}
