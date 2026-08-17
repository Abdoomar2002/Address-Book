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
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .ToListAsync(cancellationToken);
        }

        public async Task<DepartmentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dto = await _context.Departments
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
                .SingleOrDefaultAsync(cancellationToken);

            return dto ?? throw NotFound(id);
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentSaveDto dto, CancellationToken cancellationToken = default)
        {
            var name = dto.Name.Trim();
            await GuardUniqueNameAsync(name, excludeId: null, cancellationToken);

            var entity = new Department { Name = name };

            _context.Departments.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new DepartmentDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<DepartmentDto> UpdateAsync(Guid id, DepartmentSaveDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Departments.SingleOrDefaultAsync(d => d.Id == id, cancellationToken)
                ?? throw NotFound(id);

            var name = dto.Name.Trim();
            await GuardUniqueNameAsync(name, excludeId: id, cancellationToken);

            entity.Name = name;
            await _context.SaveChangesAsync(cancellationToken);

            return new DepartmentDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Departments.SingleOrDefaultAsync(d => d.Id == id, cancellationToken)
                ?? throw NotFound(id);

            // The FK is Restrict, so the database would reject this with an opaque SqlException.
            // Count first so the caller gets a 409 that explains what is blocking the delete.
            var referencingContacts = await _context.Contacts
                .CountAsync(c => c.DepartmentId == id, cancellationToken);

            if (referencingContacts > 0)
            {
                throw new ConflictException(
                    $"Department '{entity.Name}' cannot be deleted because {referencingContacts} " +
                    $"contact{(referencingContacts == 1 ? " is" : "s are")} still assigned to it.");
            }

            _context.Departments.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task GuardUniqueNameAsync(string name, Guid? excludeId, CancellationToken cancellationToken)
        {
            // Name carries a unique index; check up front rather than letting SaveChanges fail.
            var taken = await _context.Departments
                .AnyAsync(d => d.Name == name && (excludeId == null || d.Id != excludeId), cancellationToken);

            if (taken)
            {
                throw new ConflictException($"A department named '{name}' already exists.");
            }
        }

        private static KeyNotFoundException NotFound(Guid id) =>
            new KeyNotFoundException($"Department '{id}' was not found.");
    }
}
