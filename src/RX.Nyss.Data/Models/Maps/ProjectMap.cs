﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RX.Nyss.Data.Models.Maps
{
    public class ProjectMap : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.State).HasMaxLength(50).HasConversion<string>();
            builder.Property(x => x.TimeZone).HasMaxLength(50);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate);
            builder.HasOne(x => x.NationalSociety).WithMany().IsRequired().OnDelete(DeleteBehavior.Restrict);
        }
    }
}
