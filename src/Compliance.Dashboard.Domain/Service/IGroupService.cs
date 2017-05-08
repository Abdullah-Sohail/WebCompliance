using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Dashboard.Domain.Service
{
    public interface IGroupService
    {
        ICollection<Group> GetByDashProfileId(Guid profileId);
    }
}
