using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Domain;
using Compliance.Audio.Domain.Service;
using Compliance.Audio.Implementation.Ef.Persistence.Data;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Audio.Implementation.Ef.Services
{
    public class AgentLoginService : IAgentLoginService
    {
        private readonly IGenericRepo<AgentLogin, RecordingContext> _alRepo;

        public AgentLoginService(IGenericRepo<AgentLogin, RecordingContext> alRepo)
        {
            _alRepo = alRepo;
        }
        public ICollection<AgentLogin> GetAgentLogins()
        {
            return _alRepo.GetAll().ToList();
        }
    }
}
