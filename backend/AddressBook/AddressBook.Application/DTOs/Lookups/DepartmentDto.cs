using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.DTOs.Lookups
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
