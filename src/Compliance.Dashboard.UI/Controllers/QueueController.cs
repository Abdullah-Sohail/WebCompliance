using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Compliance.Dashboard.UI.Models.Queues;
using Compliance.Dashboard.UI.Models.Shared;
using Compliance.Queuing.Domain.Service;
using Compliance.Queuing.Domain.ValueTypes;
using Compliance.ScoreCards.Api;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.WorkItems.Domain;
using Compliance.WorkItems.Domain.Service;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.Dashboard.UI.Controllers
{
    public class QueueController : QueueControllerBase
    {
        private readonly IRecordingItemService _rService;
        private readonly IPullQueueService _pService;
        private readonly IApiCalls _api;

        public QueueController(IRecordingItemService rService, IPullQueueService pService, IApiCalls api, IScoreCardApi scoreCardApi) : base(scoreCardApi)
        {
            _rService = rService;
            _pService = pService;
            _api = api;
        }

        public ActionResult Index(string team, string queue)
        {
            ViewBag.ActiveMenu = queue;

            return View();
        }

        public ActionResult Work(string team, string queue, int level, string id = "")
        {
            ViewBag.ActiveMenu = queue;

            var doRedirect = false;
            var item = GetTheItem(queue, level, id, out doRedirect);

            if (doRedirect)
            {
                return RedirectToRoute("QueueWork", new
                {
                    action = "work",
                    team = team,
                    queue = queue,
                    level = level,
                    id = item.Id
                });
            }


            var results = new List<ScoreCardResultDto>();
            var reviews = new List<ScoreCardReviewDto>();
            var model = GetWorkModel(item, false, out results, out reviews);

            return View(model);
        }

        public ActionResult Review(string team, string queue, int level, string id = "")
        {
            ViewBag.ActiveMenu = queue;

            var doRedirect = false;
            var item = GetTheItem(queue, level, id, out doRedirect);

            if (doRedirect)
            {
                return RedirectToRoute("QueueWork", new
                {
                    action = "review",
                    team = team,
                    queue = queue,
                    level = level,
                    id = item.Id
                });
            }

            var results = new List<ScoreCardResultDto>();
            var reviews = new List<ScoreCardReviewDto>();

            var workModel = GetWorkModel(item, true, out results, out reviews);

            var model = new PullQueueReviewItemModel()
            {
                MyQueueItem = workModel.MyQueueItem,
                MyRecordingItem = workModel.MyRecordingItem,
                RelatedAccounts = workModel.RelatedAccounts,
                ScoreCardResults = results,
                ScoreCardReviews = reviews,
                ScoreCards = workModel.ScoreCards,
                UtcPulled = workModel.UtcPulled
            };

            return View(model);
        }

        private PullQueueItem GetTheItem(string queue, int level, string id, out bool doRedirect)
        {
            Guid gId;
            PullQueueItem item;
            var thisQueue = base.PullQueues.Where(q => q.QueueName.ToLower() == queue.ToLower()).FirstOrDefault();

            //If the id was not passed in, we need to get the next id from the pull queue
            if (!Guid.TryParse(id, out gId))
            {
                item = thisQueue.MyPullQueueItems
                    .Where(i => i.MinLevelOfConcern == level)
                    .OrderByDescending(i => i.UtcCreated).FirstOrDefault();
                doRedirect = true;
                return item;
            }

            //okay, the id was passed in, Get the item
            item = thisQueue.MyPullQueueItems
                    .Where(i => i.Id == gId)
                    .FirstOrDefault();

            //Remove it from the global queue so no one else gets it.
            thisQueue.MyPullQueueItems.Remove(item);

            //The item might have already been removed from the global queue. If so, get it from the service
            if (item == null)
                item = _pService.GetItem(thisQueue.Id, gId);

            doRedirect = false;
            return item;
        }

        private PullQueueWorkItemModel GetWorkModel(PullQueueItem item, bool fillAux, out List<ScoreCardResultDto> results, out List<ScoreCardReviewDto> reviews)
        {
            var model = new PullQueueWorkItemModel();

            //If we make it here, fill in the item
            model.MyQueueItem = item;
            model.MyRecordingItem = _rService.GetById(item.MyWorkItemId);
            model.ScoreCards = base.ScoreCards;

            var theirNum = model.MyRecordingItem.TheirNumber.Trim();

            //TODO: There is no error handling on the tasks!!!
            //Fire off 3 calls to get the audio file, get the account info from our services and mark the item as viewing

            var updateTask = Task.Factory.StartNew(() =>
            {
                var ret = _api.QueueItemAction(item.Id, "view", ViewBag.Profile.Id, "");
            });

            var fileTask = Task.Factory.StartNew(() =>
            {
                var username = User.Identity.Name.Substring(11);
                _api.GetAudioFile(model.MyRecordingItem.MyRecordingFile.FileName,
                    Server.MapPath(String.Format("~/Audio/{0}.mp3", username)));
            });

            var accountsTask = Task.Factory.StartNew(() =>
            {
                model.RelatedAccounts = _api.GetAccountInfo(theirNum, model.MyRecordingItem.CallDateTime);
            })
            .ContinueWith((atask) =>
            {
                SaveRelationships(model.MyRecordingItem, model.RelatedAccounts.ToList());
            });

            if (fillAux)
            {

                var resultApi = new ScoreCardResultApi("", "");
                var reviewApi = new ScoreCardReviewApi("", "");
                var innerResults = new List<ScoreCardResultDto>();
                var innerReviews = new List<ScoreCardReviewDto>();

                var t1 = Task.Factory.StartNew(() =>
                {
                    innerReviews = reviewApi.GetByWorkItemId(model.MyRecordingItem.Id).ScoreCardReviews.ToList();
                });

                var t2 = Task.Factory.StartNew(() =>
                {
                    innerResults = resultApi.GetByWorkItemId(model.MyRecordingItem.Id).ScoreCardResults.ToList();
                });

                Task.WaitAll(new Task[] { fileTask, accountsTask, updateTask, t1, t2 });
                reviews = innerReviews;
                results = innerResults;
            }
            else
            {
                Task.WaitAll(new Task[] { fileTask, accountsTask, updateTask });
                reviews = null;
                results = null;
            }

            if (theirNum.Length == 10)
                theirNum = String.Format("({0}) {1}-{2}", theirNum.Substring(0, 3), theirNum.Substring(3, 3), theirNum.Substring(6, 4));

            var title = model.MyRecordingItem.Incoming ? "Incoming: " : "Outgoing: ";

            title += theirNum + " on " + model.MyRecordingItem.CallDateTime.ToString("MM/dd/yyyy h:mm tt");

            ViewBag.Title = title;
            ViewBag.QueueItemId = item.Id;

            return model;
        }

	    private void SaveRelationships(RecordingItem rItem, List<AccountInfo> relatedAccounts)
        {
            //relate phones
            //relate people
            //relate desk
            //relate customer
            //relate company
            //relate account

            foreach (var acct in relatedAccounts)
            {
                foreach (var phone in acct.RelatedPhones)
                    if (rItem.PhoneRelationships.Count == 0 || !rItem.PhoneRelationships
                        .Where(p => p.Identifier == phone.Key.ToLower().Trim()).Any())
                        rItem.PhoneRelationships.Add(
                            new PhoneRelation()
                            {
                                Id = Guid.NewGuid(),
                                MyWorkItemId = rItem.Id,
                                Identifier = phone.Key.ToLower().Trim(),
                                Comment = phone.Value,
                                AuditEntityType = "User",
                                AuditEntityId = ViewBag.Profile.Id,
                                UtcCreated = DateTime.UtcNow
                            });

                foreach (var person in acct.AccountDebtors)
                    if (rItem.PersonRelationships.Count == 0 || !rItem.PersonRelationships
                        .Where(p => p.Identifier == person.ToLower().Trim()).Any())
                        rItem.PersonRelationships.Add(new PersonRelation()
                        {
                            Id = Guid.NewGuid(),
                            MyWorkItemId = rItem.Id,
                            Identifier = person.ToLower().Trim(),
                            Comment = "",
                            AuditEntityType = "User",
                            AuditEntityId = ViewBag.Profile.Id,
                            UtcCreated = DateTime.UtcNow
                        });


                if (rItem.DeskRelationships.Count == 0 || !rItem.DeskRelationships
                    .Where(p => p.Identifier == acct.Desk.ToLower().Trim()).Any())
                    rItem.DeskRelationships.Add(new DeskRelation()
                    {
                        Id = Guid.NewGuid(),
                        MyWorkItemId = rItem.Id,
                        Identifier = acct.Desk.ToLower().Trim(),
                        Comment = "",
                        AuditEntityType = "User",
                        AuditEntityId = ViewBag.Profile.Id,
                        UtcCreated = DateTime.UtcNow
                    });

                if (rItem.ClientRelationships.Count == 0 || !rItem.ClientRelationships
                    .Where(p => p.Identifier == acct.CustomerCode.ToLower().Trim()).Any())
                    rItem.ClientRelationships.Add(new ClientRelation()
                    {
                        Id = Guid.NewGuid(),
                        MyWorkItemId = rItem.Id,
                        Identifier = acct.CustomerCode.ToLower().Trim(),
                        Comment = "",
                        AuditEntityType = "User",
                        AuditEntityId = ViewBag.Profile.Id,
                        UtcCreated = DateTime.UtcNow
                    });

                if (rItem.CompanyRelationships.Count == 0 || !rItem.CompanyRelationships
                    .Where(p => p.Identifier == acct.Branch.ToLower().Trim()).Any())
                    rItem.CompanyRelationships.Add(new CompanyRelation()
                    {
                        Id = Guid.NewGuid(),
                        MyWorkItemId = rItem.Id,
                        Identifier = acct.Branch.ToLower().Trim(),
                        Comment = "",
                        AuditEntityType = "User",
                        AuditEntityId = ViewBag.Profile.Id,
                        UtcCreated = DateTime.UtcNow
                    });

                if (rItem.AccountRelationships.Count == 0 || !rItem.AccountRelationships
                    .Where(p => p.Identifier == acct.Id.ToString()).Any())
                    rItem.AccountRelationships.Add(new AccountRelation()
                    {
                        Id = Guid.NewGuid(),
                        MyWorkItemId = rItem.Id,
                        Identifier = acct.Id.ToString(),
                        Comment = "",
                        AuditEntityType = "User",
                        AuditEntityId = ViewBag.Profile.Id,
                        UtcCreated = DateTime.UtcNow
                    });

            }

            _rService.UpdateAllRelationships(rItem);
        }
    }
}
