using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Domain.Services;

namespace Compliance.ScoreCards.Api.MvcService.Controllers
{
    public class ScoreCardReviewController : ApiController
    {
        private IScoreCardService _scService;

        public ScoreCardReviewController(IScoreCardService scService)
        {
            _scService = scService;
        }

        public HttpResponseMessage Get(string command, string version, Guid id)
        {
            switch (command.ToLower())
            {
                case "byid":
                    return GetScoreCardReviewHandler(id, version);
                case "byworkitem":
                    return GetScoreCardReviewsByWorkItemHandler(id, version);
                default:
                    var err = Request.CreateErrorResponse(
                            HttpStatusCode.Ambiguous,
                            String.Format("Unknown Command: {0}", command));

                    err.Headers.Location = new Uri(
                        Url.Route("Default", new
                        {
                            controller = "Help",
                            action = "Index"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage GetScoreCardReviewsByWorkItemHandler(Guid id, string version)
        {
            switch (version)
            {
                case "20131101":
                    return GetScoreCardReviewsByWorkItem_V1(id);
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCardReview",
                            command = "byworkitem",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage GetScoreCardReviewsByWorkItem_V1(Guid workItemId)
        {
            var domainResult = _scService.GetReviewsByWorkItem(workItemId).ToList();
            var ret = new ScoreCardReviews_GetByWorkItem_Result()
            {
                ScoreCardReviews = new List<ScoreCardReviewDto>()
            };

            foreach (var scr in domainResult)
                ret.ScoreCardReviews.Add(DtoAdapter.V1.ScoreCardReviewAdapter.MakeDto(scr));

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        private HttpResponseMessage GetScoreCardReviewHandler(Guid id, string version)
        {
            switch (version)
            {
                case "20131101":
                    return GetScoreCardReview_V1(id);
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCardReview",
                            command = "byid",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage GetScoreCardReview_V1(Guid id)
        {
            var ret = _scService;

            if (ret == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    String.Format("Item {0} deosn't exist", id.ToString()));

            throw new NotImplementedException();
        }
    }
}
