using AddressBook.Application.DTOs.Lookups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressBook.Application.Interfaces
{
    public interface IJobTitleService
    {
        Task<IReadOnlyList<JobTitleDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No job title has the given id.</exception>
        Task<JobTitleDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <exception cref="Domain.Exceptions.ConflictException">Another job title already uses the name.</exception>
        Task<JobTitleDto> CreateAsync(JobTitleSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No job title has the given id.</exception>
        /// <exception cref="Domain.Exceptions.ConflictException">Another job title already uses the name.</exception>
        Task<JobTitleDto> UpdateAsync(Guid id, JobTitleSaveDto dto, CancellationToken cancellationToken = default);

        /// <exception cref="KeyNotFoundException">No job title has the given id.</exception>
        /// <exception cref="Domain.Exceptions.ConflictException">Contacts still reference the job title.</exception>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
