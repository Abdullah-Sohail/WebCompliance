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
    public class ScoreCardReviewApi
    {
        private string _developerId;
        private string _secretKey;

        public ScoreCardReviewApi(string developerId, string secretKey)
        {
            _developerId = developerId;
            _secretKey = secretKey;
        }

        //public async Task<ScoreCardReview_Save_Review> SaveAsync(
        //    ScoreCardReviewDto scoreCardReview)
        //{
        //    var r = new ScoreCardReview_Save_Command() { 
        //        MyScoreCardReview = scoreCardReview
        //    };

        //    var response = new Utility().PostApiAsync("api/scorecardReview/save/20131101", r, _developerId, _secretKey).Review;

        //    if (response.IsSuccessStatusCode)
        //        return response.Content.ReadAsAsync<ScoreCardReview_Save_Review>().Review;

        //    throw new Exception(String.Format("HTTP {0}: {1}", response.StatusCode.ToString(), response.ReasonPhrase));
        //}

        public ScoreCardReviews_GetByWorkItem_Result GetByWorkItemId(Guid workItemId)
        {
            var response = new Utility().GetApiAsync("api/scorecardreview/ByWorkItem/20131101/" + workItemId.ToString(), _developerId, _secretKey).Result;

            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsAsync<ScoreCardReviews_GetByWorkItem_Result>().Result;

            throw new Exception(String.Format("HTTP {0}: {1}", response.StatusCode.ToString(), response.ReasonPhrase));
        }
    }
}
