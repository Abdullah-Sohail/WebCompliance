using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.WorkItems.Domain
{
    public class WorkItem
    {
        public Guid Id { get; set; }

        public string LogicalIdentifier { get; set; }

        public ICollection<AccountRelation> AccountRelationships { get; set; }
        public ICollection<ClientRelation> ClientRelationships { get; set; }
        public ICollection<CompanyRelation> CompanyRelationships { get; set; }
        public ICollection<DeskRelation> DeskRelationships { get; set; }
        public ICollection<PersonRelation> PersonRelationships { get; set; }
        public ICollection<PhoneRelation> PhoneRelationships { get; set; }
        public ICollection<WorkerRelation> WorkerRelationships { get; set; }

        public ICollection<WorkItemFile> Files { get; set; }

        public DateTime UtcCreated { get; set; }

        public WorkItem()
        {
            AccountRelationships = new List<AccountRelation>();
            ClientRelationships = new List<ClientRelation>();
            CompanyRelationships = new List<CompanyRelation>();
            DeskRelationships = new List<DeskRelation>();
            PersonRelationships = new List<PersonRelation>();
            PhoneRelationships = new List<PhoneRelation>();
            WorkerRelationships = new List<WorkerRelation>();
        }
    }
}
