using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.Enums;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain.ValueTypes;

namespace Compliance.Dashboard.Domain
{
    public class Group : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyTeamId { get; set; }
        public string GroupName { get; set; }
        public GroupTypeEnum GroupType { get; set; }
        public int LevelOfConcern { get; set; }
        public bool LevelLock { get; set; }

        public ICollection<GroupMember> GroupMembers { get; set; }
        public DateTime UtcCreated { get; set; }

        public Team MyTeam { get; set; }

        public static Group Create(Guid teamId, string name, int loc, GroupTypeEnum groupType, bool levelLock = false)
        {
            return new Group()
            {
                Id = Guid.NewGuid(),
                MyTeamId = teamId,
                GroupName = name,
                GroupType = groupType,
                LevelOfConcern = loc,
                LevelLock = levelLock,
                UtcCreated = DateTime.UtcNow
            };

        }

    }
}
