using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class NewsTagMapping : IEntityTypeConfiguration<NewsTag>
    {
        public void Configure(EntityTypeBuilder<NewsTag> builder)
        {
            builder.HasKey(c => new { c.NewsId, c.TagId });
            builder.HasOne(c => c.News)
                .WithMany(c => c.NewsTags)
                .HasForeignKey(f => f.NewsId);
            builder.HasOne(c => c.Tag)
                .WithMany(c => c.NewsTags)
                .HasForeignKey(f => f.TagId);
        }
    }
}
