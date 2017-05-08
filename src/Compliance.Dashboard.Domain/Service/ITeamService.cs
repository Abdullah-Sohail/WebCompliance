using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Dashboard.Domain.Service
{
    public interface ITeamService
    {
        Team GetById(Guid teamId);
        Team GetByName(string teamName);
        ICollection<Team> GetByDashProfileId(Guid profileId);
        IEnumerable<Team> GetAll();
        void Create(Team team);
        void Update(Team team);
        void Delete(Team team);
        void AssignQueueToTeam(Guid teamId, List<Guid> queueIds);
    }
}
