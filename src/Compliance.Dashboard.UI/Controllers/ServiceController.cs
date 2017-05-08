using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.UI.Models.ApiCommands;
using Compliance.Queuing.Domain.Service;
using Compliance.ScoreCards.Api;
using Compliance.WorkItems.Domain.Service;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.Dashboard.UI.Controllers
{
    public class ServiceController : ApiController
    {
        private IRecordingItemService _rService;
        private IPullQueueService _pService;

        public ServiceController(IRecordingItemService rService, IPullQueueService pService)
        {
            _rService = rService;
            _pService = pService;
        }

        public HttpResponseMessage Post(string command, [FromBody]object args, Guid id)
        {
            switch (command)
            {
                case "AddWorker":
                    return AddWorker(id, new JavaScriptSerializer().Deserialize<AddWorkerRelationshipsCommand>(args.ToString()));
                case "QueueItemAction":
                    return QueueItemAction(id, new JavaScriptSerializer().Deserialize<QueueItemActionCommand>(args.ToString()));
                case "SaveAnswers":
                    return SaveAnswers(id, new JavaScriptSerializer().Deserialize<SaveScoreCardCommand>(args.ToString()));
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.Ambiguous, String.Format("Unknown Command: {0}", args));
            }
        }

        private HttpResponseMessage SaveAnswers(Guid scoreCardId, SaveScoreCardCommand cmd)
        {

            if (scoreCardId != cmd.MyScoreCardResult.MyScoreCardId)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");

            var api = new ScoreCardResultApi("", "");

            var cmdResult = api.SaveAsync(cmd.MyScoreCardResult).Result;

            var qiaCmd = new QueueItemActionCommand()
            {
                ActionString = "complete",
                AuditEntityId = cmd.MyScoreCardResult.AuditEntityId,
                AuditEntityType = cmd.MyScoreCardResult.AuditEntityType,
                Comment = "",
                ItemId = cmd.MyQueueItemId,
                NewLevel = 0
            };

            var ret = QueueItemAction(cmd.MyQueueItemId, qiaCmd);

            var thisCard = CacheHelper.GetScoreCards(System.Web.HttpContext.Current).Where(sc => sc.Id == scoreCardId).FirstOrDefault();

            int nextLevel = 0;

            foreach (var assReview in cmdResult.MyScoreCardReviewDto.AssertionReviews.Where(ar => ar.IsPassed == false).ToList())
            {
                var failedAss = thisCard.Assertions.Where(ass => ass.Id == assReview.MyAssertionId).First();
                nextLevel = (failedAss.EscalationLevelOfConcern > nextLevel) ? failedAss.EscalationLevelOfConcern : nextLevel;
            }

            qiaCmd = new QueueItemActionCommand()
            {
                AuditEntityId = cmd.MyScoreCardResult.AuditEntityId,
                AuditEntityType = cmd.MyScoreCardResult.AuditEntityType,
                Comment = "",
                ItemId = cmd.MyQueueItemId,
                NewLevel = nextLevel
            };

            qiaCmd.ActionString = (nextLevel > 1) ? "escalate" : "close";

            ret = QueueItemAction(cmd.MyQueueItemId, qiaCmd);

            return ret;
        }

        private HttpResponseMessage AddWorker(Guid workItemId, AddWorkerRelationshipsCommand addWorkerRelationshipsCommand)
        {
            var theItem = _rService.GetById(workItemId);

            foreach (var r in addWorkerRelationshipsCommand.Relationships)
                if (!theItem.WorkerRelationships.Where(i => i.Identifier == r.Identifier).Any())
                    _rService.AddRelationship<WorkerRelation>(theItem.Id, r);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private HttpResponseMessage QueueItemAction(Guid id, QueueItemActionCommand cmd)
        {
            if (id != cmd.ItemId)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID mismatch");
            try
            {
                switch (cmd.ActionString.ToLower())
                {
                    case "view":
                        _pService.Item_View(cmd.ItemId, "User", cmd.AuditEntityId, cmd.Comment);
                        break;
                    case "assign":
                        _pService.Item_Assign(cmd.ItemId, "User", cmd.AuditEntityId, cmd.Comment);
                        break;
                    case "complete":
                        _pService.Item_CompleteScoreCard(cmd.ItemId, "User", cmd.AuditEntityId, cmd.Comment);
                        break;
                    case "escalate":
                        _pService.Item_Escalate(cmd.ItemId, cmd.NewLevel.Value, "User", cmd.AuditEntityId, cmd.Comment);
                        break;
                    case "close":
                        _pService.Item_Close(cmd.ItemId, "User", cmd.AuditEntityId, cmd.Comment);
                        break;
                    default:
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, String.Format("Unknown Action String: {0}", cmd.ActionString));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);

        }
    }
}
