using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Audio.Domain.ValueTypes
{
    public class CustomerReference
    {
        public Guid MyRecordingId { get; set; }
        public string CustomerCode { get; set; }

        public Recording MyRecording { get; set; }
    }
}
