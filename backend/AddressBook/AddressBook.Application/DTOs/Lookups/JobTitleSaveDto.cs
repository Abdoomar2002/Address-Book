using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AddressBook.Application.DTOs.Lookups
{
    public class JobTitleSaveDto
    {
        /// <summary>Maps to <c>JobTitle.Title</c>.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must be at most 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
