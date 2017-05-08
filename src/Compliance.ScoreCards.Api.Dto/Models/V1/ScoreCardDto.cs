using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.ScoreCards.Api.Dto.Models.V1
{
    public class ScoreCardDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ICollection<AssertionDto> Assertions { get; set; }
        public int Version { get; set; }
        public Guid? MyLastVersionId { get; set; }
        public DateTime UtcCreated { get; set; }
    }
}
