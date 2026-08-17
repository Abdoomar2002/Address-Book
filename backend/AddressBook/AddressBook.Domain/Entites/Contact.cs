using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Domain.Entites
{
    public class Contact : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string PhotoBase64 { get; set; }
        public Guid JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
