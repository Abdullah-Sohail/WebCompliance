using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Compliance.Dashboard.UI.Models.Shared
{
    public class InfoRequest
    {
        public string ForPhone { get; set; }
        public DateTime TheDate { get; set; }

    }
    public class AccountNote
    {
        public int AccountId { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
        public string Alias { get; set; }
        public string RealName { get; set; }
        public DateTime Created { get; set; }
    }

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