using AddressBook.Application.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AddressBook.Application.DTOs.Contacts
{
    public class ContactSaveDto
    {
        /// <summary>Split into <c>Contact.FirstName</c> and <c>Contact.LastName</c> when mapping.</summary>
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(200, ErrorMessage = "Full name must be at most 200 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job title is required.")]
        public Guid JobTitleId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public Guid DepartmentId { get; set; }

        /// <summary>Maps to <c>Contact.PhoneNumber</c>.</summary>
        [Required(ErrorMessage = "Mobile number is required.")]
        [MaxLength(20)]
        [RegularExpression(
            @"^(\+20|0020|0)?1[0125]\d{8}$",
            ErrorMessage = "Mobile number must be a valid Egyptian mobile, e.g. 01012345678 or +201012345678.")]
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>
        /// Maps to <c>Contact.BirthDate</c>. Nullable so that an omitted value is caught by
        /// <see cref="RequiredAttribute"/>; a non-nullable DateTime would silently bind to
        /// 0001-01-01, which is technically a past date and would slip through.
        /// </summary>
        [Required(ErrorMessage = "Date of birth is required.")]
        [PastDate(ErrorMessage = "Date of birth must be in the past.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(255, ErrorMessage = "Address must be at most 255 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
        [MaxLength(255, ErrorMessage = "Email must be at most 255 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Hashed before it reaches <c>Contact.PasswordHash</c>; never stored as given.</summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Photo is required.")]
        [MaxLength(1000, ErrorMessage = "Photo must be at most 1000 characters.")]
        public string PhotoBase64 { get; set; } = string.Empty;
    }
}
