using AddressBook.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Builds a signed JWT for the given user.
        /// Returns the token together with its expiry so callers do not have to
        /// re-read the configured lifetime to populate <c>AuthResponseDto.ExpiresAt</c>.
        /// </summary>
        (string Token, DateTime ExpiresAt) CreateToken(AppUser user);
    }
}
