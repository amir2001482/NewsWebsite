using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class NewsCategoryMapping : IEntityTypeConfiguration<NewsCategory>
    {
        public void Configure(EntityTypeBuilder<NewsCategory> builder)
        {
            builder.HasKey(c => new { c.NewsId, c.CategoryId });
            builder.HasOne(c => c.News)
                .WithMany(c => c.NewsCategories)
                .HasForeignKey(f => f.NewsId);
            builder.HasOne(c => c.Category)
                .WithMany(c => c.NewsCategories)
                .HasForeignKey(f => f.CategoryId);
        }
    }
}
