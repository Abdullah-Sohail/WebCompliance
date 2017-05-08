using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Compliance.Dashboard.UI.Models.ApiCommands;
using Compliance.Dashboard.UI.Models.Shared;

namespace Compliance.Dashboard.UI.Mocks
{
    public class MockCallInfoApi : IApiCalls
    {
	    private readonly string _baseAddress;

	    public MockCallInfoApi(string baseAddress)
	    {
		    _baseAddress = baseAddress;
	    }

        public void GetAudioFile(string fileToGet, string pathToSave)
        {
	        if (File.Exists(pathToSave)) File.Delete(pathToSave);

			if(File.Exists(@"J:\Temp\recording.mp3"))
				File.Copy(@"J:\Temp\recording.mp3", pathToSave);
        }

        public ICollection<AccountInfo> GetAccountInfo(string forPhone, DateTime callTime)
        {
            var thePhones = new Dictionary<string, string>();
            return new List<AccountInfo>
            {
                new AccountInfo
                {
                    Id = 12345,
                    CustomerCode = "COMCAST",
                    AccountDebtors = new List<string>
                    {
                        "Smith, John",
                        "Doe, Jane"
                    },
                    AccountNotes = new List<AccountNote>
                    {
                        new AccountNote
                        {
                            AccountId = 12345,
                            Action = "CO",
                            Alias = "Joe Crusher",
                            Comment = "This is a note within 12 hours of call either direction",
                            Created = DateTime.Parse("1/1/2016 14:01"),
                            RealName = "real name",
                            Result = "CO",
                            Username = "AgentSmith"
                        },
                        new AccountNote
                        {
                            AccountId = 12345,
                            Action = "DT",
                            Alias = "Joe Crusher",
                            Comment = "This is the actual note from the call.",
                            Created = DateTime.Parse("1/1/2016 13:01"),
                            RealName = "John Smith",
                            Result = "TT",
                            Username = "AgentSmith"
                        },
                        new AccountNote
                        {
                            AccountId = 12345,
                            Action = "CO",
                            Alias = "Jacki Chan",
                            Comment = "The system will default an eligible note to cehcked if it's within 15 minutes o fthe call time",
                            Created = DateTime.Parse("1/1/2016 11:01"),
                            RealName = "Jane Doe",
                            Result = "CO",
                            Username = "AgentJane"
                        },
                        new AccountNote
                        {
                            AccountId = 12345,
                            Action = "CO",
                            Alias = "alias",
                            Comment = "Notice that notes without proper action/result codes are not selectable",
                            Created = DateTime.Parse("1/1/2016 12:01"),
                            RealName = "real name",
                            Result = "CO",
                            Username = "AgentSmith"
                        }
                    },
                    Branch = "BRACNH1",
                    CurrentBalance = 25.34m,
                    CustAccountNum = "COM3424",
                    CustomerName = "Comcast Cable",
                    DateClosed = null,
                    DateLastPaid = null,
                    DateReceived = DateTime.Parse("1/23/2015"),
                    DateReturned = null,
                    Desk = "DESK1",
                    OriginalBalance = 25.34m,
                    OriginalCreditor = "Comcast",
                    RelatedPhones = thePhones
                }
            };
        }

        public HttpResponseMessage QueueItemAction(Guid itemId, string action, Guid userId, string comment, int newLevel = 0)
        {
            var cmd = new QueueItemActionCommand()
            {
                ActionString = action,
                AuditEntityId = userId,
                AuditEntityType = "User",
                Comment = comment,
                ItemId = itemId,
                NewLevel = newLevel
            };

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("http://compliance.ccscollect.com/");
                client.BaseAddress = new Uri(_baseAddress);
                var res = client.PostAsJsonAsync("/api/service/queueitemaction/" + itemId.ToString(), cmd);
                return res.Result;
            }
        }
    }
}