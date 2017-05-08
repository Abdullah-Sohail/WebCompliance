using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Dashboard.Domain.ValueTypes
{
    public class GroupMember : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyDashProfileId { get; set; }
        public Guid MyGroupId { get; set; }
        public DateTime UtcCreated { get; set; }

        public Group MyGroup { get; set; }
        public DashProfile MyDashProfile { get; set; }
    }
}
