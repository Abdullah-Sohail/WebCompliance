using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Compliance.Dashboard.UI.Models.Shared
{
    public class QueueMenuItem
    {
        public int Order { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public string Action { get; set; }
        public bool Active { get; set; }
        public List<QueueMenuItem> Children { get; set; }
    }
}