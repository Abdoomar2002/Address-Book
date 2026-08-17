using AddressBook.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Infrastructure.Data.Seed
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        public DbSeeder(AppDbContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            // Seed data for AppUser
            if (!_context.AppUsers.Any())
            {
                var users = new List<AppUser>
                {
                    // password : "admin123"
                    new AppUser { FullName = "Abdo Omar", Email = "abdo@gmail.com", PasswordHash ="AQAAAAIAAYagAAAAEGSUBv7LaieIYWjClySw7Ya/7R5VVasssK7MVjB6M7kkl9Yj/NGizazpT0VmedwseA==" },
                   

                };
                var departments = new List<Department>
                {
                    new Department { Name = "HR" },
                    new Department { Name = "IT" }
                };
                var jobs = new List<JobTitle>
                {
                    new JobTitle { Title = "Manager" },
                    new JobTitle { Title = "Developer" }
                };

                _context.AppUsers.AddRange(users);
                _context.Departments.AddRange(departments);
                _context.JobTitles.AddRange(jobs);
                _context.SaveChanges();
            }
        }
    }
}
