using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class LikeMapping : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(c => new { c.NewsId, c.IpAddress });
            builder.HasOne(c => c.News)
                .WithMany(c => c.Likes)
                .HasForeignKey(f => f.NewsId);
        }
    }
}
