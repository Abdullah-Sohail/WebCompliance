using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Compiance.Audio.Host.AspNetCore.Models
{
	public partial class LatitudeController : Controller
    {
		public class AccountInfo
        {
            public int Id { get; set; }
            public List<string> AccountDebtors { get; set; }
            public Dictionary<string, string> RelatedPhones { get; set; }
            public List<AccountNote> AccountNotes { get; set; }
            public string Branch { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string OriginalCreditor { get; set; }
            public string CustAccountNum { get; set; }
            public string Desk { get; set; }
            public DateTime DateReceived { get; set; }
            public DateTime? DateClosed { get; set; }
            public DateTime? DateReturned { get; set; }
            public DateTime? DateLastPaid { get; set; }
            public decimal OriginalBalance { get; set; }
            public decimal CurrentBalance { get; set; }
            //original/current/
        }
    }
}
