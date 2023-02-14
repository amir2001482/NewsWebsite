using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class BookmarkMapping : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.HasKey(t => new { t.UserId, t.NewsId });
            builder
              .HasOne(p => p.News)
              .WithMany(t => t.Bookmarks)
              .HasForeignKey(f => f.NewsId);

            builder
               .HasOne(p => p.User)
               .WithMany(t => t.Bookmarks)
               .HasForeignKey(f => f.UserId);
        }
    }
}
