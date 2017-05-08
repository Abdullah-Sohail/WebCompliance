using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain.ValueType;
using Compliance.Dashboard.Domain.ValueTypes;

namespace Compliance.Dashboard.Domain
{
    public class Team : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public string TeamName { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<QueueAssignment> QueueAssignments { get; set; }
        public ICollection<ScoreCardAssignment> ScoreCardAssignments { get; set; }
        public DateTime UtcCreated { get; set; }
        public bool IsActive { get; set; }

        public static Team Create(string teamName)
        {
            return new Team()
            {
                Id = Guid.NewGuid(),
                TeamName = teamName,
                UtcCreated = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
