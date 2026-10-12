using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime CreatedAt { get; private set; }
        public ICollection<Order> Orders { get; private set; } = new List<Order>();
        public ICollection<Address> Addresses { get; private set; } = new List<Address>();

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
            ValidateDetails(
                firstName,
                lastName,
                email,
                phoneNumber,
                dateOfBirth);

            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            this.CreatedAt = DateTime.UtcNow;
        }

        private static void ValidateDetails(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber,
            DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException(
                    "First name is required.",
                    nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException(
                    "Last name is required.",
                    nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(email) ||
                !new EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException(
                    "A valid email address is required.",
                    nameof(email));
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new ArgumentException(
                    "Phone number is required.",
                    nameof(phoneNumber));
            }

            if (dateOfBirth.Date > DateTime.UtcNow.Date)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(dateOfBirth),
                    "Date of birth cannot be in the future.");
            }
        }

        public void UpdateDetails(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber,
            DateTime dateOfBirth)
        {
            ValidateDetails(
                firstName,
                lastName,
                email,
                phoneNumber,
                dateOfBirth);

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }
    }
}