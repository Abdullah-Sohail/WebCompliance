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
    public class RecordingService : IRecordingService
    {
        private readonly IGenericRepo<Recording, RecordingContext> _recordingRepo;

        private readonly string[] _includes = new string[] { 
            "AccountReferences",
            "CustomerReferences",
            "DeskReferences",
            "UserReferences",
            "ResultReferences"
        };

        public RecordingService(IGenericRepo<Recording, RecordingContext> recordingRepo)
        {
            _recordingRepo = recordingRepo;
        }

        public Recording GetById(Guid id)
        {
            return _recordingRepo.Query(r => r.Id == id, _includes).FirstOrDefault();
        }

        public ICollection<Recording> GetByPhone(string phone)
        {
            return _recordingRepo.Query(r => r.TheirNumber == phone, _includes).ToList();
        }

        public ICollection<Recording> GetByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds)
        {
            return _recordingRepo.Query(r =>
                r.CallStart >= startDate
                && r.CallStart <= endDate
                && r.CallDuration >= minSeconds
                && r.CallDuration <= maxSeconds
                , _includes).ToList();
        }

        public ICollection<Recording> GetByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds, List<KeyValuePair<Domain.Enum.FilterTypeEnum, string>> filters, bool includeOnly = false)
        {
            var ret = _recordingRepo.Query(r =>
                r.CallStart >= startDate
                && r.CallStart <= endDate
                && r.CallDuration >= minSeconds
                && r.CallDuration <= maxSeconds
                , _includes);

            if (!includeOnly)
                foreach (var kvp in filters)
                {
                    switch (kvp.Key)
                    {
                        case Compliance.Audio.Domain.Enum.FilterTypeEnum.Account:
                            int acctnum;
                            int.TryParse(kvp.Value, out acctnum);

                            ret = ret.Where(r => !r.AccountReferences.Where(ar => ar.AccountNumber == acctnum).Any());

                            break;
                        case Compliance.Audio.Domain.Enum.FilterTypeEnum.Customer:

                            ret = ret.Where(r => !r.CustomerReferences.Where(ar => ar.CustomerCode == kvp.Value).Any());

                            break;
                        case Compliance.Audio.Domain.Enum.FilterTypeEnum.Desk:

                            ret = ret.Where(r => !r.DeskReferences.Where(ar => ar.DeskCode == kvp.Value).Any());

                            break;
                        case Compliance.Audio.Domain.Enum.FilterTypeEnum.User:

                            ret = ret.Where(r => !r.UserReferences.Where(ar => ar.UserLogin == kvp.Value).Any());

                            break;
                        case Compliance.Audio.Domain.Enum.FilterTypeEnum.ResultCode:

                            ret = ret.Where(r => !r.ResultReferences.Where(ar => ar.ResultCode == kvp.Value).Any());

                            break;
                        default:
                            throw new Exception(string.Format("Unhandled FilterTypeEnum {0}", kvp.Key.ToString()));
                    }
                }
            else
            {

                if (filters.Where(f => f.Key == Domain.Enum.FilterTypeEnum.Account).Any())
                    ret = ret.Where(r =>
                        r.AccountReferences.Where(ar =>
                            filters.Where(af =>
                                af.Key == Domain.Enum.FilterTypeEnum.Account && af.Value == ar.AccountNumber.ToString()
                                ).Any()
                            ).Any()
                        );
                if (filters.Where(f => f.Key == Domain.Enum.FilterTypeEnum.Customer).Any())
                    ret = ret.Where(r =>
                        r.CustomerReferences.Where(ar =>
                            filters.Where(af =>
                                af.Key == Domain.Enum.FilterTypeEnum.Customer && af.Value == ar.CustomerCode
                                ).Any()
                            ).Any()
                        );
                if (filters.Where(f => f.Key == Domain.Enum.FilterTypeEnum.Desk).Any())
                    ret = ret.Where(r =>
                        r.DeskReferences.Where(ar =>
                            filters.Where(af =>
                                af.Key == Domain.Enum.FilterTypeEnum.Desk && af.Value == ar.DeskCode
                                ).Any()
                            ).Any()
                        );
                if (filters.Where(f => f.Key == Domain.Enum.FilterTypeEnum.User).Any())
                    ret = ret.Where(r =>
                        r.UserReferences.Where(ar =>
                            filters.Where(af =>
                                af.Key == Domain.Enum.FilterTypeEnum.User && af.Value == ar.UserLogin
                                ).Any()
                            ).Any()
                        );
                if (filters.Where(f => f.Key == Domain.Enum.FilterTypeEnum.ResultCode).Any())
                    ret = ret.Where(r =>
                        r.ResultReferences.Where(ar =>
                            filters.Where(af =>
                                af.Key == Domain.Enum.FilterTypeEnum.ResultCode && af.Value == ar.ResultCode
                                ).Any()
                            ).Any()
                        );
            }

            return ret.ToList();
        }

        public int CountByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds)
        {
            return _recordingRepo.Query(r =>
                r.CallStart >= startDate
                && r.CallStart <= endDate
                && r.CallDuration >= minSeconds
                && r.CallDuration <= maxSeconds
                ).Count();
        }

        public int CountByDateDuration(DateTime startDate, DateTime endDate, int minSeconds, int maxSeconds, List<KeyValuePair<Domain.Enum.FilterTypeEnum, string>> filters, bool includeOnly = false)
        {
            var ret = _recordingRepo.Query(r =>
                r.CallStart >= startDate
                && r.CallStart <= endDate
                && r.CallDuration >= minSeconds
                && r.CallDuration <= maxSeconds
                , _includes);

            foreach (var kvp in filters)
            {
                switch (kvp.Key)
                {
                    case Compliance.Audio.Domain.Enum.FilterTypeEnum.Account:
                        int acctnum;
                        int.TryParse(kvp.Value, out acctnum);

                        ret = ret.Where(r => r.AccountReferences.Where(ar => ar.AccountNumber == acctnum).Any() == includeOnly);

                        break;
                    case Compliance.Audio.Domain.Enum.FilterTypeEnum.Customer:

                        ret = ret.Where(r => r.CustomerReferences.Where(ar => ar.CustomerCode == kvp.Value).Any() == includeOnly);

                        break;
                    case Compliance.Audio.Domain.Enum.FilterTypeEnum.Desk:

                        ret = ret.Where(r => r.DeskReferences.Where(ar => ar.DeskCode == kvp.Value).Any() == includeOnly);

                        break;
                    case Compliance.Audio.Domain.Enum.FilterTypeEnum.User:

                        ret = ret.Where(r => r.UserReferences.Where(ar => ar.UserLogin == kvp.Value).Any() == includeOnly);

                        break;
                    case Compliance.Audio.Domain.Enum.FilterTypeEnum.ResultCode:

                        ret = ret.Where(r => r.ResultReferences.Where(ar => ar.ResultCode == kvp.Value).Any() == includeOnly);

                        break;
                    default:
                        throw new Exception(string.Format("Unhandled FilterTypeEnum {0}", kvp.Key.ToString()));
                }
            }

            return ret.Count();
        }
    }
}
