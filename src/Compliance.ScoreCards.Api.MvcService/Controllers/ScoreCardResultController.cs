using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Api.DtoAdapter.V1;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.Services;

namespace Compliance.ScoreCards.Api.MvcService.Controllers
{
    public class ScoreCardResultController : ApiController
    {
        private IScoreCardService _scService;

        public ScoreCardResultController(IScoreCardService scService)
        {
            _scService = scService;
        }

        public HttpResponseMessage Get(string command, string version, Guid id)
        {
            switch (command.ToLower())
            {
                case "byid":
                    return GetScoreCardResultHandler(id, version);
                case "byworkitem":
                    return GetScoreCardResultsByWorkItemHandler(id, version);
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

        private HttpResponseMessage GetScoreCardResultsByWorkItemHandler(Guid id, string version)
        {
            switch (version)
            {
                case "20131101":
                    return GetScoreCardResultsByWorkItem_V1(id);
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCardResult",
                            command = "byworkitem",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage GetScoreCardResultsByWorkItem_V1(Guid workItemId)
        {
            var domainResult = _scService.GetResultsByWorkItem(workItemId).ToList();
            var ret = new ScoreCardResults_GetByWorkItem_Result(){
                ScoreCardResults = new List<ScoreCardResultDto>()
            };

            foreach (var scr in domainResult)
                ret.ScoreCardResults.Add(DtoAdapter.V1.ScoreCardResultAdapter.MakeDto(scr));

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        private HttpResponseMessage GetScoreCardResultHandler(Guid id, string version)
        {
            switch (version)
            {
                case "20131101":
                    return GetScoreCardResult_V1(id);
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCardResult",
                            command = "byid",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage GetScoreCardResult_V1(Guid id)
        {
            var ret = _scService;

            if (ret == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, 
                    String.Format("Item {0} deosn't exist", id.ToString()));

            throw new NotImplementedException();
        }

        public HttpResponseMessage Post([FromBody]object serialCmd, string command, string version)
        {

            switch (command.ToLower())
            {
                case "save":
                    return SaveScoreCardResultHandler(serialCmd, version);
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

        private HttpResponseMessage SaveScoreCardResultHandler(object serialCmd, string version)
        {
            switch (version)
            {
                case "20131101":
                    return SaveScoreCardResult_V1(new JavaScriptSerializer().Deserialize<ScoreCardResult_Save_Command>(serialCmd.ToString()));
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCardResult",
                            command = "save",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        private HttpResponseMessage SaveScoreCardResult_V1(ScoreCardResult_Save_Command cmd)
        {
            var retContent = new ScoreCardResult_Save_Result();

            try
            {
                var newResult = ScoreCardResultAdapter.MakeDomain(cmd.MyScoreCardResult);

                newResult = _scService.SaveScoreCardResult(newResult);

                var theReview = _scService.GenerateReview(newResult);

                //Build Dto response
                retContent.MyScoreCardResultDto = ScoreCardResultAdapter.MakeDto(newResult);

                retContent.MyScoreCardReviewDto = ScoreCardReviewAdapter.MakeDto(theReview);
                
                var ret = Request.CreateResponse(HttpStatusCode.Created, retContent);

                //TODO: Add Location Header to get results in the future to be RESTful
                //ret.Headers.Location 

                return ret;
            }
            catch (Exception ex)
            {
                var msg = String.Format("Err: {0}", ex.Message);

                if (ex.InnerException != null)
                    msg += String.Format(" Inner: {0}", ex.InnerException.Message);
                if (ex.InnerException.InnerException != null)
                    msg += String.Format(" DB: {0}", ex.InnerException.InnerException.Message);

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
            }

        }
    }
}
