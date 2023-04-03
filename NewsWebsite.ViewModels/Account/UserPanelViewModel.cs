using NewsWebsite.Entities.identity;
using NewsWebsite.ViewModels.News;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.ViewModels.Account
{
    public class UserPanelViewModel
    {
        public UserPanelViewModel(User user, List<NewsViewModel> bookmarks)
        {
            User = user;
            Bookmarks = bookmarks;
        }
        public User User { get; set; }
        public List<NewsViewModel> Bookmarks { get; set; }
    }
}
