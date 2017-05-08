using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.UI.Models.ApiCommands;
using Compliance.Dashboard.UI.Models.Shared;

namespace Compliance.Dashboard.UI
{
    public class ApiCalls : IApiCalls
    {
        public async void GetAudioFile(string fileToGet, string pathToSave)
        {
            //User.Identity.Name.Substring(11);
            //Server.MapPath(String.Format("~/Audio/{0}.mp3", username));

            if (System.IO.File.Exists(pathToSave))
                System.IO.File.Delete(pathToSave);

            var fileStream = new FileStream(pathToSave, FileMode.Create, FileAccess.Write, FileShare.None);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.ccscollect.com");
                var res = client.PostAsJsonAsync("/latitude/api/values", fileToGet);

                await res.Result.Content.CopyToAsync(fileStream);

                fileStream.Close();
            }
        }

        public ICollection<AccountInfo> GetAccountInfo(string forPhone, DateTime callTime)
        {
            var ret = new List<AccountInfo>();
            var req = new InfoRequest()
            {
                ForPhone = forPhone,
                TheDate = callTime
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.ccscollect.com");
                var res = client.PostAsJsonAsync("/latitude/api/latitude", req);
                ret = res.Result.Content.ReadAsAsync<ICollection<AccountInfo>>().Result.ToList();
            }

            return ret;
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
                client.BaseAddress = new Uri("http://compliance.ccscollect.com/");
                //client.BaseAddress = new Uri("http://localhost:4019/"); 
                var res = client.PostAsJsonAsync("/api/service/queueitemaction/" + itemId.ToString(), cmd);
                return res.Result;
            }
        }
    }
}