using AddressBook.Application.DTOs.Lookups;
using AddressBook.Application.Interfaces;
using AddressBook.Domain.Entites;
using AddressBook.Domain.Exceptions;
using AddressBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressBook.Infrastructure.Services
{
    public class JobTitleService : IJobTitleService
    {
        private readonly AppDbContext _context;

        public JobTitleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<JobTitleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.JobTitles
                .AsNoTracking()
                .OrderBy(j => j.Title)
                .Select(j => new JobTitleDto { Id = j.Id, Name = j.Title })
                .ToListAsync(cancellationToken);
        }

        public async Task<JobTitleDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dto = await _context.JobTitles
                .AsNoTracking()
                .Where(j => j.Id == id)
                .Select(j => new JobTitleDto { Id = j.Id, Name = j.Title })
                .SingleOrDefaultAsync(cancellationToken);

            return dto ?? throw NotFound(id);
        }

        public async Task<JobTitleDto> CreateAsync(JobTitleSaveDto dto, CancellationToken cancellationToken = default)
        {
            var name = dto.Name.Trim();
            await GuardUniqueNameAsync(name, excludeId: null, cancellationToken);

            var entity = new JobTitle { Title = name };

            _context.JobTitles.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new JobTitleDto { Id = entity.Id, Name = entity.Title };
        }

        public async Task<JobTitleDto> UpdateAsync(Guid id, JobTitleSaveDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.JobTitles.SingleOrDefaultAsync(j => j.Id == id, cancellationToken)
                ?? throw NotFound(id);

            var name = dto.Name.Trim();
            await GuardUniqueNameAsync(name, excludeId: id, cancellationToken);

            entity.Title = name;
            await _context.SaveChangesAsync(cancellationToken);

            return new JobTitleDto { Id = entity.Id, Name = entity.Title };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.JobTitles.SingleOrDefaultAsync(j => j.Id == id, cancellationToken)
                ?? throw NotFound(id);

            // The FK is Restrict, so the database would reject this with an opaque SqlException.
            // Count first so the caller gets a 409 that explains what is blocking the delete.
            var referencingContacts = await _context.Contacts
                .CountAsync(c => c.JobTitleId == id, cancellationToken);

            if (referencingContacts > 0)
            {
                throw new ConflictException(
                    $"Job title '{entity.Title}' cannot be deleted because {referencingContacts} " +
                    $"contact{(referencingContacts == 1 ? " is" : "s are")} still assigned to it.");
            }

            _context.JobTitles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task GuardUniqueNameAsync(string name, Guid? excludeId, CancellationToken cancellationToken)
        {
            // Title carries a unique index; check up front rather than letting SaveChanges fail.
            var taken = await _context.JobTitles
                .AnyAsync(j => j.Title == name && (excludeId == null || j.Id != excludeId), cancellationToken);

            if (taken)
            {
                throw new ConflictException($"A job title named '{name}' already exists.");
            }
        }

        private static KeyNotFoundException NotFound(Guid id) =>
            new KeyNotFoundException($"Job title '{id}' was not found.");
    }
}
