using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.DTOs.Lookups
{
    public class JobTitleDto
    {
        public Guid Id { get; set; }

        /// <summary>Maps from <c>JobTitle.Title</c>.</summary>
        public string Name { get; set; } = string.Empty;
    }
}
