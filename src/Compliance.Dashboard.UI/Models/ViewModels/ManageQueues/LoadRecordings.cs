using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Compliance.Dashboard.UI.Models.ViewModels.ManageQueues
{
    public class LoadRecordings
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int? ResultCount { get; set; }
        public bool IncludeOnly { get; set; }
        public Guid? AddToQueueId { get; set; }
        public ICollection<SummaryRow> CustomerCodes { get; set; }
        public ICollection<SummaryRow> DeskCodes { get; set; }
        public ICollection<SummaryRow> UserCodes { get; set; }
        public ICollection<SummaryRow> AllResultCodes { get; set; }
        public ICollection<SummaryRow> AllAgents { get; set; }
        public ICollection<string> SelectedResultCodes { get; set; }
        public ICollection<string> SelectedAgents { get; set; }
    }

    public class SummaryRow
    {
        public bool Checked { get; set; }
        public string LabelValue { get; set; }
        public int CountValue { get; set; }
    }
}