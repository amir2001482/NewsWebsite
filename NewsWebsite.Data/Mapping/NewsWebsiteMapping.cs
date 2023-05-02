using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public static class NewsWebsiteMapping
    {
        public static void AddCustomNewsWebsiteMapping(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BookmarkMapping());
            builder.ApplyConfiguration(new LikeMapping());
            builder.ApplyConfiguration(new NewsCategoryMapping());
            builder.ApplyConfiguration(new NewsTagMapping());
            builder.ApplyConfiguration(new VisitMapping());
            builder.ApplyConfiguration(new CommentMapping());
            builder.ApplyConfiguration(new NewsMapping());

        }
    }
}
