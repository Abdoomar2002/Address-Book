using AddressBook.Application.DTOs.Contacts;
using AddressBook.Application.Interfaces;
using AddressBook.Domain.Entites;
using AddressBook.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AddressBook.Infrastructure.Services
{
    public class ContactService : IContactService
    {
        // FirstName and LastName are each capped at 100 by ContactConfiguration.
        private const int MaxNamePartLength = 100;

        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Contact> _passwordHasher;

        public ContactService(AppDbContext context, IPasswordHasher<Contact> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IReadOnlyList<ContactDto>> GetAllAsync(
            ContactFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new ContactFilterDto();

            var query = _context.Contacts
                .Include(c => c.JobTitle)
                .Include(c => c.Department)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();

                query = query.Where(c =>
                    (c.FirstName + " " + c.LastName).Contains(term) ||
                    c.Email.Contains(term) ||
                    c.PhoneNumber.Contains(term) ||
                    c.Address.Contains(term) ||
                    c.JobTitle.Title.Contains(term) ||
                    c.Department.Name.Contains(term));
            }

            if (filter.JobTitleId.HasValue)
            {
                query = query.Where(c => c.JobTitleId == filter.JobTitleId.Value);
            }

            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(c => c.DepartmentId == filter.DepartmentId.Value);
            }

            if (filter.BirthDateFrom.HasValue)
            {
                query = query.Where(c => c.BirthDate >= filter.BirthDateFrom.Value);
            }

            if (filter.BirthDateTo.HasValue)
            {
                query = query.Where(c => c.BirthDate <= filter.BirthDateTo.Value);
            }

            var contacts = await query
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync(cancellationToken);

            return contacts.Select(MapToDto).ToList();
        }

        public async Task<ContactDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contact = await _context.Contacts
                .Include(c => c.JobTitle)
                .Include(c => c.Department)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

            return contact is null ? throw NotFound(id) : MapToDto(contact);
        }

        public async Task<ContactDto> CreateAsync(ContactSaveDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new Contact();
            await ApplyAsync(entity, dto, isCreate: true, cancellationToken);

            _context.Contacts.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Re-read so JobTitleName and DepartmentName reflect the saved foreign keys.
            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<ContactDto> UpdateAsync(Guid id, ContactSaveDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Contacts.SingleOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw NotFound(id);

            await ApplyAsync(entity, dto, isCreate: false, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Contacts.SingleOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw NotFound(id);

            _context.Contacts.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Copies a save DTO onto an entity, hashing the password. Shared by create and update
        /// so the two can never drift apart.
        /// </summary>
        private async Task ApplyAsync(Contact entity, ContactSaveDto dto, bool isCreate, CancellationToken cancellationToken)
        {
            if (dto.DateOfBirth is null)
            {
                throw new ArgumentException("Date of birth is required.");
            }

            if (isCreate && string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            await GuardLookupsExistAsync(dto.JobTitleId, dto.DepartmentId, cancellationToken);

            var (firstName, lastName) = SplitFullName(dto.FullName);

            entity.FirstName = firstName;
            entity.LastName = lastName;
            entity.Email = dto.Email.Trim();
            entity.PhoneNumber = dto.MobileNumber.Trim();
            entity.BirthDate = dto.DateOfBirth.Value;
            entity.Address = dto.Address.Trim();
            entity.PhotoBase64 = dto.PhotoBase64;
            entity.JobTitleId = dto.JobTitleId;
            entity.DepartmentId = dto.DepartmentId;

            // Only rehash when a password was supplied, so an edit that leaves the field
            // blank keeps the existing one. The plain value is never persisted.
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                entity.PasswordHash = _passwordHasher.HashPassword(entity, dto.Password);
            }
        }

        /// <summary>
        /// Both foreign keys are Restrict-enforced; checking here turns an opaque
        /// SqlException into a 400 that names the offending id.
        /// </summary>
        private async Task GuardLookupsExistAsync(Guid jobTitleId, Guid departmentId, CancellationToken cancellationToken)
        {
            if (!await _context.JobTitles.AnyAsync(j => j.Id == jobTitleId, cancellationToken))
            {
                throw new ArgumentException($"Job title '{jobTitleId}' does not exist.");
            }

            if (!await _context.Departments.AnyAsync(d => d.Id == departmentId, cancellationToken))
            {
                throw new ArgumentException($"Department '{departmentId}' does not exist.");
            }
        }

        /// <summary>
        /// The DTO carries one name, the entity stores two. Splits on the first space:
        /// everything after it becomes the last name.
        /// </summary>
        private static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var trimmed = fullName.Trim();
            var separator = trimmed.IndexOf(' ');

            var firstName = separator < 0 ? trimmed : trimmed.Substring(0, separator);
            var lastName = separator < 0 ? string.Empty : trimmed.Substring(separator + 1).Trim();

            if (firstName.Length > MaxNamePartLength || lastName.Length > MaxNamePartLength)
            {
                throw new ArgumentException(
                    $"First and last name must each be at most {MaxNamePartLength} characters.");
            }

            return (firstName, lastName);
        }

        private static ContactDto MapToDto(Contact contact) => new ContactDto
        {
            Id = contact.Id,
            FullName = $"{contact.FirstName} {contact.LastName}".Trim(),
            Email = contact.Email,
            MobileNumber = contact.PhoneNumber,
            DateOfBirth = contact.BirthDate,
            Address = contact.Address,
            JobTitleId = contact.JobTitleId,
            JobTitleName = contact.JobTitle?.Title ?? string.Empty,
            DepartmentId = contact.DepartmentId,
            DepartmentName = contact.Department?.Name ?? string.Empty,
            PhotoBase64 = contact.PhotoBase64
            // Age is computed from DateOfBirth; never assigned.
        };

        private static KeyNotFoundException NotFound(Guid id) =>
            new KeyNotFoundException($"Contact '{id}' was not found.");
    }
}
