using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.Domain.ValueType;
using Compliance.Queuing.Domain;

namespace Compliance.Dashboard.Implementation.Ef.Services
{
    public class TeamService : ITeamService
    {
        private IGenericRepo<Team, DashboardContext> _teamRepo;
        private IGenericRepo<QueueAssignment, DashboardContext> _queueAssignmentRepo;

        private string[] _valueIncludes;

        public TeamService(IGenericRepo<Team, DashboardContext> teamRepo, IGenericRepo<QueueAssignment, DashboardContext> queueAssignmentRepo)
        {
            _teamRepo = teamRepo;
            _queueAssignmentRepo = queueAssignmentRepo;
            _valueIncludes = new string[] { "QueueAssignments", "ScoreCardAssignments" };
        }

        public Team GetById(Guid teamId)
        {
            return _teamRepo.Query(p => p.Id == teamId, _valueIncludes).FirstOrDefault();
        }

        public void Create(Team team)
        {
            //Validation
            if (string.IsNullOrWhiteSpace(team.TeamName))
                throw new Exception("Cannot create team without name.");

            _teamRepo.Add(team);
            _teamRepo.Save();
        }

        public void AssignQueueToTeam(Guid teamId, List<Guid> queueIds) 
        {
            var theTeam = GetById(teamId);
            var existedQueueAssignment = _queueAssignmentRepo.Query(t => t.MyTeamId==teamId);
            foreach(var queueAssignment in existedQueueAssignment) {

                _queueAssignmentRepo.Remove(queueAssignment);
            }

            //_queueAssignmentRepo.Save();
            DateTime createdOn = DateTime.Now;

            foreach (var queue in queueIds)
            {
                var assignment = new QueueAssignment
                {
                    Id = Guid.NewGuid(),
                    MyQueueId = queue,
                    MyTeamId = teamId,
                    UtcCreated = createdOn
                };
                _queueAssignmentRepo.Add(assignment);
            }

            _queueAssignmentRepo.Save();
            _teamRepo.Save();
        }

        public void Delete(Team team)
        {
            var theTeam = GetById(team.Id);
            theTeam.IsActive = false;
            theTeam.QueueAssignments = null;
            _teamRepo.Update(theTeam);
            
            _teamRepo.Save();
        }

        public IEnumerable<Team> GetAll()
        {
            return _teamRepo.Query(t => t.IsActive == true, _valueIncludes);
        }

        public ICollection<Team> GetByDashProfileId(Guid profileId)
        {
            return _teamRepo
                .Query(t => t.Groups
                    .Where(g => g.GroupMembers
                        .Where(m => m.MyDashProfileId == profileId)
                        .Any())
                    .Any(),
                    _valueIncludes)
                .ToList();
        }
        

        public void Update(Team team)
        {
            var theTeam = GetById(team.Id);
            theTeam.TeamName = team.TeamName;
            _teamRepo.Update(theTeam);
            _teamRepo.Save();
        }
        

        public Team GetByName(string teamName)
        {
            return _teamRepo.Query(p => p.TeamName == teamName,_valueIncludes).FirstOrDefault();
        }
    }
}
