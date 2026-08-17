using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Domain.Exceptions
{
    /// <summary>
    /// Raised when a request is well formed but conflicts with the current state of the data,
    /// such as deleting a lookup that other records still reference. Surfaces as HTTP 409.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
