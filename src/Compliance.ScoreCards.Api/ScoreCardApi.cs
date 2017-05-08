using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Compliance.ScoreCards.Api.Dto.Packages.V1;

namespace Compliance.ScoreCards.Api
{
    public class ScoreCardApi : IScoreCardApi
	{        
        private string _developerId;
        private string _secretKey;

        public ScoreCardApi(string developerId, string secretKey)
        {
            _developerId = developerId;
            _secretKey = secretKey;
        }

        public async Task<ScoreCard_GetAll_Result> GetAllAsync()
        {
	        var response = await new Utility().GetApiAsync("api/scorecard/all/20131101", _developerId, _secretKey);
            string ret = String.Empty;

            if (response.IsSuccessStatusCode)
                ret = response.Content.ReadAsStringAsync().Result;
            else
                throw new Exception(String.Format("HTTP {0}: {1}", response.StatusCode.ToString(), response.ReasonPhrase));

            return new JavaScriptSerializer().Deserialize<ScoreCard_GetAll_Result>(ret);
        }
    }
}
