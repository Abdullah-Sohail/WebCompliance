using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;

namespace Compliance.Dashboard.Implementation.Ef.Services
{
    public class GroupService : IGroupService
    {

        private IGenericRepo<Group, DashboardContext> _groupRepo;
        private string[] _valueIncludes;

        public GroupService(IGenericRepo<Group, DashboardContext> groupRepo)
        {
            _groupRepo = groupRepo;
            _valueIncludes = new string[] { "GroupMembers" };
        }

        public ICollection<Group> GetByDashProfileId(Guid profileId)
        {
            return _groupRepo
                .Query(g => g.GroupMembers
                    .Where(m => m.MyDashProfileId == profileId)
                    .Any(), _valueIncludes)
                .ToList();
        }
    }
}
