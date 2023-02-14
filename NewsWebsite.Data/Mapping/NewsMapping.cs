using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class NewsMapping : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.HasKey(c => c.NewsId);
            builder.HasOne(c => c.User)
                .WithMany(c => c.News)
                .HasForeignKey(c => c.UserId);

        }
    }
}
