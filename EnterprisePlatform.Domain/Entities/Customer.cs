using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }

        public string FirstName { get; private set; } = string.Empty;

        public string LastName { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string PhoneNumber { get; private set; } = string.Empty;

        public DateTime DateOfBirth { get; private set; }

        public long Version { get; private set; }

        private Customer()
        {
            // Required by EF Core
        }

        public Customer(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            DateTime dateOfBirth)
        {
            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }

        public void UpdateDetails(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber,
            DateTime dateOfBirth)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }
    }
}