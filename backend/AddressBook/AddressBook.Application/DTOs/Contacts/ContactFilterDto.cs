using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.DTOs.Contacts
{
    /// <summary>
    /// Query parameters for listing contacts. Every member is optional; an empty instance
    /// means "no filtering".
    /// </summary>
    public class ContactFilterDto
    {
        /// <summary>Free-text match, intended against name, email and mobile number.</summary>
        public string? SearchTerm { get; set; }

        public Guid? JobTitleId { get; set; }

        public Guid? DepartmentId { get; set; }

        /// <summary>Inclusive lower bound on <c>Contact.BirthDate</c>.</summary>
        public DateTime? BirthDateFrom { get; set; }

        /// <summary>Inclusive upper bound on <c>Contact.BirthDate</c>.</summary>
        public DateTime? BirthDateTo { get; set; }
    }
}
