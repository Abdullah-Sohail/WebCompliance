using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compliance.Audio.Domain;
using Compliance.Audio.Domain.Enum;
using Compliance.Audio.Domain.Service;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.UI.Models.ViewModels.ManageQueues;
using Compliance.Queuing.Domain.Service;
using Compliance.Queuing.Domain.ValueTypes;
using Compliance.WorkItems.Domain;
using Compliance.WorkItems.Domain.Service;
using Compliance.WorkItems.Domain.ValueTypes;

namespace Compliance.Dashboard.UI.Controllers
{
    public class ManageQueueController : ControllerBase
    {
        private readonly IRecordingService _rService;
        private readonly IPullQueueService _pqService;
        private readonly IRecordingItemService _riService;
        private readonly IAgentLoginService _aService;

        private readonly string[] filterCodes = new string[] { "DC", "HU", "LM", "LR", "NM", "RF", "T1", "TT", "TW", "WN" };
        public ManageQueueController(IRecordingService rService, IPullQueueService pqService, IRecordingItemService riService, IAgentLoginService aService)
        {
            _rService = rService;
            _riService = riService;
            _pqService = pqService;
            _aService = aService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Load()
        {
            var model = new LoadRecordings()
            {
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today,
                MinLength = 2,
                MaxLength = 10,
                ResultCount = 0,
                IncludeOnly = false,
                AllResultCodes = new List<SummaryRow>(),
                SelectedResultCodes = new List<string>(),
                AllAgents = new List<SummaryRow>(),
                SelectedAgents = new List<string>()
            };


            foreach (var code in filterCodes)
                model.AllResultCodes.Add(new SummaryRow()
                {
                    Checked = false,
                    CountValue = 0,
                    LabelValue = code
                });

            foreach (var agent in _aService.GetAgentLogins())
                model.AllAgents.Add(new SummaryRow()
                {
                    Checked = false,
                    CountValue = 0,
                    LabelValue = agent.UserLogin
                });

            return View(model);
        }

        [HttpPost]
        public JsonResult Load(LoadRecordings model)
        {
            var filters = new List<KeyValuePair<FilterTypeEnum, string>>();

            //Add pre select filters
            if (model.SelectedResultCodes != null)
                foreach (var r in model.SelectedResultCodes)
                    filters.Add(new KeyValuePair<FilterTypeEnum, string>(FilterTypeEnum.ResultCode, r));

            if (model.SelectedAgents != null)
                foreach (var a in model.SelectedAgents)
                    filters.Add(new KeyValuePair<FilterTypeEnum, string>(FilterTypeEnum.User, a));

            //If pre select filters are in play, search must be IncludeOnly
            if (filters.Any() && !model.IncludeOnly) model.IncludeOnly = true;

            //Get the rest of the filters from the tables.
            if (model.CustomerCodes != null)
                foreach (var c in model.CustomerCodes)
                    if (c.Checked == model.IncludeOnly)
                        filters.Add(new KeyValuePair<FilterTypeEnum, string>(FilterTypeEnum.Customer, c.LabelValue));
            
            if (model.DeskCodes != null)
                foreach (var d in model.DeskCodes)
                    if (d.Checked == model.IncludeOnly)
                        filters.Add(new KeyValuePair<FilterTypeEnum, string>(FilterTypeEnum.Desk, d.LabelValue));
            
            if (model.UserCodes != null)
                foreach (var u in model.UserCodes)
                    if (u.Checked == model.IncludeOnly)
                        filters.Add(new KeyValuePair<FilterTypeEnum, string>(FilterTypeEnum.User, u.LabelValue));


            var results = _rService.GetByDateDuration(model.StartDate, model.EndDate, model.MinLength * 60, model.MaxLength * 60, filters, model.IncludeOnly);

            model.ResultCount = results.Count;

            if (filters.Count == 0)
            {
                model.CustomerCodes = new List<SummaryRow>();
                model.DeskCodes = new List<SummaryRow>();
                model.UserCodes = new List<SummaryRow>();
            }
            else
            {
                model.CustomerCodes.ToList().ForEach(c => c.CountValue = 0);
                model.DeskCodes.ToList().ForEach(c => c.CountValue = 0);
                model.UserCodes.ToList().ForEach(c => c.CountValue = 0);
            }

            foreach (var r in results)
            {
                foreach (var c in r.CustomerReferences)
                {
                    var thisRec = model.CustomerCodes.Where(x => x.LabelValue == c.CustomerCode).FirstOrDefault();

                    if (thisRec == null)
                    {
                        model.CustomerCodes.Add(new SummaryRow()
                        {
                            Checked = true,
                            CountValue = 1,
                            LabelValue = c.CustomerCode
                        });
                    }
                    else
                    {
                        thisRec.CountValue += 1;
                    }
                }
                foreach (var c in r.DeskReferences)
                {
                    var thisRec = model.DeskCodes.Where(x => x.LabelValue == c.DeskCode).FirstOrDefault();

                    if (thisRec == null)
                    {
                        model.DeskCodes.Add(new SummaryRow()
                        {
                            Checked = true,
                            CountValue = 1,
                            LabelValue = c.DeskCode
                        });
                    }
                    else
                    {
                        thisRec.CountValue += 1;
                    }
                }
                foreach (var c in r.UserReferences)
                {
                    var thisRec = model.UserCodes.Where(x => x.LabelValue == c.UserLogin).FirstOrDefault();

                    if (thisRec == null)
                    {
                        model.UserCodes.Add(new SummaryRow()
                        {
                            Checked = true,
                            CountValue = 1,
                            LabelValue = c.UserLogin
                        });
                    }
                    else
                    {
                        thisRec.CountValue += 1;
                    }
                }
            }

            model.CustomerCodes = model.CustomerCodes.OrderBy(c => c.LabelValue).ToList();
            model.DeskCodes = model.DeskCodes.OrderBy(d => d.LabelValue).ToList();
            model.UserCodes = model.UserCodes.OrderBy(u => u.LabelValue).ToList();


            if (model.AddToQueueId.HasValue)
            {
                AddToQueue(model.AddToQueueId.Value, results);

                model = new LoadRecordings()
                {
                    StartDate = DateTime.Today.AddDays(-1),
                    EndDate = DateTime.Today,
                    MinLength = 2,
                    MaxLength = 10,
                    ResultCount = 0,
                    AddToQueueId = model.AddToQueueId
                };

            }

            return Json(model);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        private void AddToQueue(Guid qId, ICollection<Recording> recordings)
        {
            foreach (var r in recordings)
            {
                var wId = Guid.NewGuid();
                var fId = Guid.NewGuid();

                var riFile = new WorkItemFile()
                {
                    Id = fId,
                    Comment = "",
                    Expires = r.CallStart.AddYears(2),
                    FileName = r.Filename,
                    FilePath = r.Filename,
                    MediaType = "audio/mp3",
                    MinLevelOfConcern = 1,
                    MyWorkItemId = wId,
                    UtcCreated = DateTime.UtcNow
                };

                var rItem = new RecordingItem()
                {
                    Id = wId,
                    CallDateTime = r.CallStart,
                    CallDuration = r.CallDuration,
                    Files = new List<WorkItemFile>() { riFile },
                    Incoming = r.Incoming,
                    LogicalIdentifier = r.CallIdentity,
                    MyRecordingFileId = riFile.Id,
                    OurNumber = r.OurNumber,
                    TheirNumber = r.TheirNumber,
                    UtcCreated = DateTime.UtcNow
                };

                _riService.Add(rItem);

                var pqItem = new PullQueueItem()
                {
                    Id = Guid.NewGuid(),
                    MinLevelOfConcern = 1,
                    MyPullQueueId = qId,
                    MyWorkItemId = wId,
                    UtcCreated = DateTime.UtcNow
                };

                _pqService.Item_AddToQueue(pqItem, "User", ((DashProfile)ViewBag.Profile).Id, "");

            }

            this.HttpContext.Application["NextRefresh"] = DateTime.UtcNow.AddMinutes(-1);

        }
    }
}
