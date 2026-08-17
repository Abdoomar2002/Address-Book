using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Domain.Entites
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
