using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Domain.Enum;

namespace Compliance.Audio.Domain.Service
{
    public interface IRecordingService
    {
        Recording GetById(Guid id);
        ICollection<Recording> GetByPhone(string phone);
        ICollection<Recording> GetByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds);
        ICollection<Recording> GetByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds, List<KeyValuePair<FilterTypeEnum, string>> filters, bool includeOnly = false);
        int CountByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds);
        int CountByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds, List<KeyValuePair<FilterTypeEnum, string>> filters, bool includeOnly = false);
    }
}
