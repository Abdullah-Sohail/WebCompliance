using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using Compliance.ScoreCards.Api.DtoAdapter.V1;
using Compliance.ScoreCards.Domain.Services;

namespace Compliance.ScoreCards.Api.MvcService.Controllers
{
    public class ScoreCardController : ApiController
    {
        private IScoreCardService _scService;

        public ScoreCardController(IScoreCardService scService)
        {
            _scService = scService;
        }

        public HttpResponseMessage Get(string command, string version, [FromBody]object serialCmd)
        { 
            switch (command.ToLower())
            {
                case "all":
                    return GetAllScoreCardHandler(version);
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

        protected HttpResponseMessage GetAllScoreCardHandler(string version)
        {
            switch (version)
            {
                case "20131101":
                    return GetAllScoreCards_V1();
                default:
                    //Return Ambiguous and point location to latest version
                    var err = Request.CreateErrorResponse(
                        HttpStatusCode.Ambiguous,
                        String.Format("Unknown Version: {0}", version));

                    err.Headers.Location = new Uri(
                        Url.Route("DefaultApi", new
                        {
                            controller = "ScoreCard",
                            command = "all",
                            version = "20131101"
                        }));

                    return err;
            }
        }

        protected HttpResponseMessage GetAllScoreCards_V1()
        {
            var retContent = new ScoreCard_GetAll_Result(){
                ScoreCardDtos = new List<ScoreCardDto>()
            };
            
            foreach(var sc in _scService.GetAll())
                retContent.ScoreCardDtos.Add(ScoreCardAdapter.MakeDto(sc));

            return Request.CreateResponse(HttpStatusCode.OK, retContent);
        }
    }
}
