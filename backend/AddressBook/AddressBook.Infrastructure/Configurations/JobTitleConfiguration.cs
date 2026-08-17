using AddressBook.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Infrastructure.Configurations
{
    public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.Property(j => j.Title).IsRequired().HasMaxLength(100);
            builder.HasIndex(j => j.Title).IsUnique();
        }
    }
}
