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
    public class QueueLevelConfigService : IQueueLevelConfigService
    {
        private IGenericRepo<QueueLevelConfig, DashboardContext> _queueLevelConfigRepo;

        public QueueLevelConfigService(IGenericRepo<QueueLevelConfig, DashboardContext> queueLevelConfigRepo)
        {
            _queueLevelConfigRepo = queueLevelConfigRepo;
        }

        public ICollection<Domain.QueueLevelConfig> GetAll()
        {
            return _queueLevelConfigRepo.GetAll().ToList();    
        }
    }
}
