using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class CommentMapping : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.CommentId);
            builder.HasOne(c => c.News)
                .WithMany(c => c.Comments)
                .HasForeignKey(f => f.NewsId);
            builder.HasOne(c => c.comment)
                .WithMany(c => c.comments)
                .HasForeignKey(f => f.ParentCommentId);

        }
    }
}
