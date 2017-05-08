using System;
using System.Collections.Generic;
using System.Net.Http;
using Compliance.Dashboard.UI.Models.Shared;

namespace Compliance.Dashboard.UI
{
    public interface IApiCalls
    {
        void GetAudioFile(string fileToGet, string pathToSave);
        ICollection<AccountInfo> GetAccountInfo(string forPhone, DateTime callTime);
        HttpResponseMessage QueueItemAction(Guid itemId, string action, Guid userId, string comment, int newLevel = 0);
    }
}