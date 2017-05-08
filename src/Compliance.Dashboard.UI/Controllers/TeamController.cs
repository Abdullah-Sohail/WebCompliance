using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Compliance.Queuing.Domain.Service;
using Compliance.Dashboard.Domain.ValueType;

namespace Compliance.Dashboard.UI.Controllers
{
    public class TeamController : Controller
    {
        private ITeamService _teamService;
        private readonly IPullQueueService _pService;
        public TeamController()
        {
            _teamService = DependencyResolver.Current.GetService<ITeamService>();
            _pService = DependencyResolver.Current.GetService<IPullQueueService>();
        }

        private TeamPaginationViewModel GetPaginatedTeams(int? page, string search, string sortField, bool? isAsc)
        {
            var teams = _teamService.GetAll().Select(o => new TeamViewModel
            {
                Id = o.Id,
                TeamName = o.TeamName,
                IsActive = o.IsActive,
                UtcCreated = o.UtcCreated,
                TeamQueues = o.QueueAssignments == null ? 0 : o.QueueAssignments.Count(),
                TeamMembers = o.Groups == null ? 0 : o.Groups.Select(g => g.GroupMembers).Count()
            });

            #region search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                teams = teams
                        .Where(m => (!string.IsNullOrEmpty(m.TeamName) && m.TeamName.ToLower().Contains(search)));
            }
            #endregion

            #region Sorting
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                Func<TeamViewModel, string> orderingFunction = (c => sortField == "Id" ? c.Id.ToString() :
                                                                    sortField == "TeamName" ? c.TeamName :
                                                                    sortField == "TeamMembers" ? c.TeamMembers.ToString() :
                                                                    sortField == "TeamQueues" ? c.TeamQueues.ToString() :
                                                                    c.TeamName);

                if (isAsc.HasValue && isAsc.Value)
                    teams = teams.OrderBy(orderingFunction);
                else
                    teams = teams.OrderByDescending(orderingFunction);

            }
            #endregion

            #region pagination
            var pager = new Pager(teams.Count(), page);

            var viewModel = new TeamPaginationViewModel
            {
                Items = teams.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize).AsEnumerable(),
                Pager = pager
            };
            #endregion
            return viewModel;
        }

        public JsonResult AllQueue() {
            var queue = _pService.GetActive();
            return Json(queue,JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateQueue(TeamViewModel model)
        {
            try
            {
                //var team = _teamService.GetById(model.Id);

                _teamService.AssignQueueToTeam(model.Id, model.SelectedQueues);


                //DateTime createdOn = DateTime.Now;
                //_teamService.ClearQueues(team);




                //List<QueueAssignment> queues = new List<QueueAssignment>();
                //foreach(var queue in model.SelectedQueues) 
                //{
                //    var assignment = new QueueAssignment
                //    {
                //    Id=Guid.NewGuid(),
                //        MyQueueId = queue,
                //        MyTeamId = model.Id,
                //        UtcCreated = createdOn
                //    };
                //    queues.Add(assignment);
                //}
                
               
                //    team.QueueAssignments = queues;
                //    _teamService.Update(team);
                  var viewModel = GetPaginatedTeams(1, null, null, null);
                    return Json(new { IsSuccess = true, data = viewModel, msg = "Team name has been udpate." });
               
            }
            catch(Exception ex)
            {
                return Json(new { IsSuccess = false, msg = "Request failed." });
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page, string search, string sortField, bool? isAsc)
        {
            ViewBag.ActiveMenu = "Teams";
            var viewModel = GetPaginatedTeams(page, search, sortField, isAsc);

            if (Request.IsAjaxRequest())
            {
                return Json(viewModel, JsonRequestBehavior.AllowGet);
            }
            return View(viewModel);
        }


        [Authorize(Roles = "Admin")]
        // POST: Role/Create
        [HttpPost]
        public JsonResult Create(TeamViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var team = _teamService.GetByName(model.TeamName);
                    if (team != null)
                    {
                        return Json(new { IsSuccess = false, msg = "Team already exists!" });
                    }
                    _teamService.Create(Team.Create(model.TeamName));
                    var viewModel = GetPaginatedTeams(1, null, null, null);
                    return Json(new { IsSuccess = true, data = viewModel });
                }
                return Json(new { IsSuccess = false, msg = "Team name can not be empty!" });
            }
            catch
            {
                return Json(new { IsSuccess = false, msg = "Request failed." });
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: Team/Edit/5
        [HttpPost]
        public JsonResult Edit(TeamViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var team = _teamService.GetByName(model.TeamName);
                    if (team != null)
                    {
                        return Json(new { IsSuccess = false, msg = "Team already exists!" });
                    }
                    else
                    {
                        team = _teamService.GetById(model.Id);
                        team.TeamName = model.TeamName;
                        _teamService.Update(team);
                        var viewModel = GetPaginatedTeams(1, null, null, null);
                        return Json(new { IsSuccess = true, data = viewModel, msg = "Team name has been udpate." });
                    }
                }
                return Json(new { IsSuccess = false, msg = "Team name can not be empty!" });
            }
            catch
            {
                return Json(new { IsSuccess = false, msg = "Request failed." });
            }
        }

        [Authorize(Roles = "Admin")]
        // POST: Role/Delete/5
        [HttpPost]
        public JsonResult Delete(TeamViewModel model)
        {
            try
            {
                var team = _teamService.GetById(model.Id);
                if (team == null)
                {
                    return Json(new { IsSuccess = false, msg = "Team does not exists." });
                }
                _teamService.Delete(team);
                var viewModel = GetPaginatedTeams(1, null, null, null);
                return Json(new { IsSuccess = true, data = viewModel, msg = "Team has been deleted." });
            }
            catch
            {
                return Json(new { IsSuccess = false, msg = "Request failed." });
            }
        }
    }
}
