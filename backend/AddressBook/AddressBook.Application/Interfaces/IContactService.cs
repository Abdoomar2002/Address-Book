using AddressBook.Application.DTOs.Contacts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressBook.Application.Interfaces
{
    public interface IContactService
    {
        Task<IReadOnlyList<ContactDto>> GetAllAsync(ContactFilterDto filter, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No contact has the given id.</exception>
        Task<ContactDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <exception cref="ArgumentException">The referenced job title or department does not exist.</exception>
        Task<ContactDto> CreateAsync(ContactSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No contact has the given id.</exception>
        /// <exception cref="ArgumentException">The referenced job title or department does not exist.</exception>
        Task<ContactDto> UpdateAsync(Guid id, ContactSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No contact has the given id.</exception>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
