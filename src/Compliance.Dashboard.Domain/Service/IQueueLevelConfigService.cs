using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Dashboard.Domain;

namespace Compliance.Dashboard.Domain.Service
{
    public interface IQueueLevelConfigService
    {
        ICollection<QueueLevelConfig> GetAll();
    }
}
