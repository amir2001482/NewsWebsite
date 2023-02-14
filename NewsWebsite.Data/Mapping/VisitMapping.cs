using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsWebsite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Data.Mapping
{
    public class VisitMapping : IEntityTypeConfiguration<Visit>
    {
        public void Configure (EntityTypeBuilder<Visit> builder)
        {
            builder.HasKey(c => new { c.IpAddress, c.NewsId });
            builder.HasOne(c => c.News)
                .WithMany(c => c.Visits)
                .HasForeignKey(f => f.NewsId);
        }
    }
}
