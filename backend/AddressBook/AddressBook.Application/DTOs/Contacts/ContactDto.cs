using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.DTOs.Contacts
{
    /// <summary>
    /// Read model for a contact. Deliberately carries no password field of any kind.
    /// </summary>
    public class ContactDto
    {
        public Guid Id { get; set; }

        /// <summary>Composed from <c>Contact.FirstName</c> and <c>Contact.LastName</c>.</summary>
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>Maps from <c>Contact.PhoneNumber</c>.</summary>
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>Maps from <c>Contact.BirthDate</c>.</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Whole years elapsed since <see cref="DateOfBirth"/>, computed rather than stored
        /// so it can never go stale. Read-only; it still serializes into the JSON response.
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;

                // Birthday has not come round yet this year.
                if (DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }

        public string Address { get; set; } = string.Empty;

        public Guid JobTitleId { get; set; }

        /// <summary>Maps from <c>Contact.JobTitle.Title</c>.</summary>
        public string JobTitleName { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }

        /// <summary>Maps from <c>Contact.Department.Name</c>.</summary>
        public string DepartmentName { get; set; } = string.Empty;

        public string PhotoBase64 { get; set; } = string.Empty;
    }
}
