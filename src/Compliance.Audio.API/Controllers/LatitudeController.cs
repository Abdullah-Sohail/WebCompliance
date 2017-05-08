using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Compliance.Audio.API.Controllers
{
    public class LatitudeController : ApiController
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

        public List<AccountInfo> Post([FromBody]InfoRequest theReq)
        {
            List<AccountInfo> accts = new List<AccountInfo>();
            using (var conSql = new SqlConnection(ConfigurationManager.ConnectionStrings["LatitudeDb"].ConnectionString))
            {
                ;
                var cmd = new SqlCommand(@"
SELECT DISTINCT  m.number ,
        desk ,
        m.Branch ,
        account ,
        received ,
        closed ,
        returned ,
        lastpaid ,
        m.status ,
        m.customer ,
        c.Name [CustName],
        original ,
        current0 ,
        OriginalCreditor
FROM    master m
        INNER JOIN dbo.Phone p ON m.number = p.AccountID
        INNER JOIN dbo.customer c on m.customer = c.customer
WHERE   p.number = @Phone
            ", conSql);

                cmd.Parameters.AddWithValue("@Phone", theReq.ForPhone);

                conSql.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        accts.Add(new AccountInfo()
                        {
                            Id = (int)rdr["number"],
                            AccountDebtors = new List<string>(),
                            RelatedPhones = new Dictionary<string, string>(),
                            AccountNotes = new List<AccountNote>(),
                            Branch = rdr["Branch"].ToString(),
                            CustomerCode = rdr["customer"].ToString(),
                            CustomerName = rdr["CustName"].ToString(),
                            OriginalCreditor = rdr["OriginalCreditor"].ToString(),
                            CustAccountNum = rdr["account"].ToString(),
                            Desk = rdr["desk"].ToString(),
                            DateReceived = (DateTime)rdr["received"],
                            DateClosed = (rdr["closed"] is DBNull) ? null : (DateTime?)rdr["closed"],
                            DateReturned = (rdr["returned"] is DBNull) ? null : (DateTime?)rdr["returned"],
                            DateLastPaid = (rdr["lastpaid"] is DBNull) ? null : (DateTime?)rdr["lastpaid"],
                            OriginalBalance = (decimal)rdr["original"],
                            CurrentBalance = (decimal)rdr["current0"],
                        });
                    }
                }

                foreach (var acct in accts)
                {
                    cmd = new SqlCommand("SELECT Name FROM Debtors WHERE number = @FileNum", conSql);
                    cmd.Parameters.AddWithValue("@FileNum", acct.Id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            acct.AccountDebtors.Add(rdr[0].ToString());
                        }
                    }

                    cmd = new SqlCommand("SELECT Number, pt.PhoneType FROM Phone p JOIN PhoneTypes pt ON p.PhoneType = pt.PhoneTypeId WHERE AccountID = @FileNum AND Status NOT IN (2,11)", conSql);
                    cmd.Parameters.AddWithValue("@FileNum", acct.Id);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            try { acct.RelatedPhones.Add(rdr[0].ToString(), rdr[1].ToString()); }
                            catch { }
                        }
                    }

                    cmd = new SqlCommand("SELECT number, action, result, comment, user0, created, Alias, UserName FROM notes LEFT OUTER JOIN Users on notes.user0 = Users.LoginName WHERE number = @FileNum AND created BETWEEN @StartDate AND @EndDate", conSql);
                    cmd.Parameters.AddWithValue("@FileNum", acct.Id);
                    cmd.Parameters.AddWithValue("@StartDate", theReq.TheDate.AddHours(-10));
                    cmd.Parameters.AddWithValue("@EndDate", theReq.TheDate.AddHours(10));
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            try {
                                acct.AccountNotes.Add(new AccountNote()
                                {
                                    AccountId = (int)rdr["number"],
                                    Action = rdr["action"].ToString(),
                                    Result = rdr["result"].ToString(),
                                    Comment = rdr["comment"].ToString(),
                                    Username = rdr["user0"].ToString(),
                                    Alias = (rdr["Alias"] is DBNull) ? "" : rdr["Alias"].ToString(),
                                    RealName = (rdr["UserName"] is DBNull) ? "" : rdr["UserName"].ToString(),
                                    Created = (DateTime)rdr["created"]
                                }); 
                            }
                            catch { }
                        }
                    }
                }
            }

            return accts;

        }
    }
}
