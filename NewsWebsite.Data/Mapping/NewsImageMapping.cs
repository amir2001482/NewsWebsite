using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class NewsImageMapping : IEntityTypeConfiguration<NewsImage>
    {
        public void Configure(EntityTypeBuilder<NewsImage> builder)
        {
            builder.HasKey(c => c.NewsImageId);
            builder.HasOne(c => c.News)
                .WithMany(c => c.NewsImages)
                .HasForeignKey(f => f.NewsId);
        }
    }
}
