using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;

namespace Compliance.ScoreCards.Api
{
    public class ScoreCardResultApi
    {
        private string _developerId;
        private string _secretKey;

        public ScoreCardResultApi(string developerId, string secretKey)
        {
            _developerId = developerId;
            _secretKey = secretKey;
        }

        public async Task<ScoreCardResult_Save_Result> SaveAsync(
            ScoreCardResultDto scoreCardResult)
        {
            var r = new ScoreCardResult_Save_Command() { 
                MyScoreCardResult = scoreCardResult
            };

            var response = new Utility().PostApiAsync("api/scorecardresult/save/20131101", r, _developerId, _secretKey).Result;

            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsAsync<ScoreCardResult_Save_Result>().Result;

            throw new Exception(String.Format("HTTP {0}: {1}", response.StatusCode.ToString(), response.ReasonPhrase));
        }

        public ScoreCardResults_GetByWorkItem_Result GetByWorkItemId(Guid workItemId)
        {
            var response = new Utility().GetApiAsync("api/scorecardresult/ByWorkItem/20131101/" + workItemId.ToString(), _developerId, _secretKey).Result;

            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsAsync<ScoreCardResults_GetByWorkItem_Result>().Result;

            throw new Exception(String.Format("HTTP {0}: {1}", response.StatusCode.ToString(), response.ReasonPhrase));
        }
    }
}
