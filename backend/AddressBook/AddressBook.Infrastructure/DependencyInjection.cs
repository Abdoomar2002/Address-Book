using AddressBook.Application.Interfaces;
using AddressBook.Domain.Entites;
using AddressBook.Infrastructure.Data;
using AddressBook.Infrastructure.Data.Seed;
using AddressBook.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Infrastructure
{
    public static class DependencyInjection 
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register infrastructure services here
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<DbSeeder>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
            services.AddScoped<IJobTitleService, JobTitleService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IPasswordHasher<Contact>, PasswordHasher<Contact>>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();
        }
    }
}
