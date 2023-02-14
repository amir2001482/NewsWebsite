using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Data.Mapping;
using NewsWebsite.Entities;
using NewsWebsite.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data
{
    public class NewsDBContext : IdentityDbContext<User,Role , string , UserClaim , UserRole , IdentityUserLogin<string> , RoleClaim , IdentityUserToken<string>>
    {
        public NewsDBContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.AddCustomIdentityMapping();
            builder.AddCustomNewsWebsiteMapping();
            builder.Entity<News>().Property(b => b.PublishDateTime).HasDefaultValueSql("CONVERT(datetime,GetDate())");
        }
        public virtual DbSet<Category> Categories { set; get; }
        public virtual DbSet<News> News { set; get; }
        public virtual DbSet<Bookmark> Bookmarks { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<NewsCategory> NewsCategories { get; set; }
        public virtual DbSet<NewsImage> NewsImages { get; set; }
        public virtual DbSet<NewsLetter> Newsletters { get; set; }
        public virtual DbSet<NewsTag> NewsTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
    }
}
