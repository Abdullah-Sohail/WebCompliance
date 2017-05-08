using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.WorkItems.Domain.ValueTypes
{
    public class Tie
    {
        public Guid Id { get; set; }
        public string EntityType1 { get; set; }
        public Guid EntityId1 { get; set; }
        public string EntityType2 { get; set; }
        public Guid EntityId2 { get; set; }
    }
}
