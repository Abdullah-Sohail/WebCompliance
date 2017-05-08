using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.Audio.Domain.ValueTypes
{
    public class UserReference
    {
        public Guid MyRecordingId { get; set; }
        public string UserLogin { get; set; }

        public Recording MyRecording { get; set; }
    }
}
