using AddressBook.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Infrastructure.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
            builder.Property(c => c.PhoneNumber).HasMaxLength(20);
            builder.Property(c => c.Address).HasMaxLength(255);
            builder.Property(c => c.PhotoBase64).HasMaxLength(1000);
            builder.HasOne(c => c.JobTitle)
                   .WithMany()
                   .HasForeignKey(c => c.JobTitleId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Department)
                     .WithMany()
                     .HasForeignKey(c => c.DepartmentId)
                     .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(c => c.Email); 
        }
    }
}
