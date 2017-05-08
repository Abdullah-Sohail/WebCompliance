using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Audio.Domain.Service
{
    public interface IAgentLoginService
    {
        ICollection<AgentLogin> GetAgentLogins();
    }
}
