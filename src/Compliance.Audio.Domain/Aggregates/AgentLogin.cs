using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Audio.Domain
{
    public class AgentLogin : ICanGenericRepo
    {
        public string UserLogin { get; set; }
    }
}
