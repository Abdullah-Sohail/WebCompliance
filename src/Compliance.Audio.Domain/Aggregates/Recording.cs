using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Audio.Domain.ValueTypes;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.Audio.Domain
{
    public class Recording : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public string CallIdentity { get; set; }
        public string OurNumber { get; set; }
        public string TheirNumber { get; set; }
        public bool Incoming { get; set; }
        public DateTime CallStart { get; set; }
        public int CallDuration { get; set; }
        public string Filename { get; set; }
        public string ArchiveFilename { get; set; }

        public ICollection<AccountReference> AccountReferences { get; set; }
        public ICollection<CustomerReference> CustomerReferences { get; set; }
        public ICollection<DeskReference> DeskReferences { get; set; }
        public ICollection<UserReference> UserReferences { get; set; }
        public ICollection<ResultReference> ResultReferences { get; set; }
    }
}
