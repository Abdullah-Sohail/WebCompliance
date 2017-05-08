using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain.ValueTypes;

namespace Compliance.Dashboard.Domain
{
    public class DashProfile : ICanGenericRepo
    {

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmailAddress Email { get; set; }
        public Phone CellNumber { get; set; }
        public DateTime UtcCreated { get; set; }
        public bool IsActive { get; set; }
        public static DashProfile Create(string userId, string firstName, string lastName, string email, string phone)
        {
            return new DashProfile() {
                Id = Guid.NewGuid(),
                UserId = new Guid(userId),
                FirstName = firstName,
                LastName = lastName,
                CellNumber = new Phone(phone),
                Email = new EmailAddress(email),
                UtcCreated = DateTime.UtcNow,
                IsActive = true
            };
        }

        public static DashProfile Create(string email)
        {
            return new DashProfile()
            {
                Id = Guid.NewGuid(),
                CellNumber = new Phone(),
                Email = new EmailAddress(email),
                UtcCreated = DateTime.UtcNow,
                IsActive=true
            };
        }
    }
}
