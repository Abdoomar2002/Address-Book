using AddressBook.Application.DTOs.Lookups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressBook.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No department has the given id.</exception>
        Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <exception cref="Domain.Exceptions.ConflictException">Another department already uses the name.</exception>
        Task<DepartmentDto> CreateAsync(DepartmentSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No department has the given id.</exception>
        /// <exception cref="Domain.Exceptions.ConflictException">Another department already uses the name.</exception>
        Task<DepartmentDto> UpdateAsync(Guid id, DepartmentSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No department has the given id.</exception>
        /// <exception cref="Domain.Exceptions.ConflictException">Contacts still reference the department.</exception>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
