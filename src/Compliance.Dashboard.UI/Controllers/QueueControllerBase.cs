using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.UI.DependencyResolution;
using Compliance.Dashboard.UI.Models;
using Compliance.Dashboard.UI.Models.Shared;
using Compliance.Queuing.Domain;
using Compliance.Queuing.Domain.ValueTypes;
using Compliance.ScoreCards.Api;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Microsoft.AspNet.Identity;
using StructureMap;

namespace Compliance.Dashboard.UI.Controllers
{
    public class QueueControllerBase : ControllerBase
    {
	    private readonly IScoreCardApi _scoreCardApi;

	    private ICollection<PullQueue> _pullQueues;
        private ICollection<ScoreCardDto> _scoreCards;
        private ICollection<Group> _groups;
        private ICollection<Team> _teams;
        private ICollection<PullQueueItem> _reviewItems;
        private ICollection<QueueLevelConfig> _levelConfigs;

        private int _loc;
        private string _theAction;

        public ICollection<PullQueue> PullQueues { get { return _pullQueues; } }
        public ICollection<ScoreCardDto> ScoreCards { get { return _scoreCards; } }

	    public QueueControllerBase(IScoreCardApi scoreCardApi)
	    {
		    _scoreCardApi = scoreCardApi;
	    }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {

                var theHandler = filterContext.HttpContext.Handler as MvcHandler;
                var currentTeam = theHandler.RequestContext.RouteData.Values["team"] as string;
                var container = DependencyResolver.Current;
                //var myProfile = CacheHelper.GetProfile(filterContext.HttpContext, container);
                var myProfile = CacheHelper.GetProfileByUserId(User.Identity.GetUserId(), container);

                //Populate private collections from cache
                RetrieveCacheValues(filterContext, container, myProfile);

                //Make sure user is on this team
                //if (!_teams.Where(t => t.TeamName == currentTeam).Any())
                //{
                //    filterContext.Result = new HttpUnauthorizedResult();
                //    return;
                //}
                
                //Get the remaining private values
                _theAction = theHandler.RequestContext.RouteData.Values["action"] as string;

                //max level authorization
                _loc = _groups
                    .Where(g => g.LevelLock == false)
                    .OrderByDescending(g => g.LevelOfConcern)
                    .FirstOrDefault()
                    .LevelOfConcern;
                //TODO: We aren't handling level locked groups.

                //Fill up the viewbag
                FillViewBag(filterContext, theHandler, currentTeam, myProfile);

                //Make sure user has authorization for this level
                if (ViewBag.Level.QueueLevel > _loc)
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                    return;
                }
            }

        }

        private void FillViewBag(ActionExecutingContext filterContext, MvcHandler theHandler, string currentTeam, DashProfile myProfile)
        {
            int level = 0;
            ViewBag.Profile = myProfile;
            ViewBag.ShowPlayer = true;
            ViewBag.QueueName = theHandler.RequestContext.RouteData.Values["queue"] as string;
            ViewBag.TeamName = theHandler.RequestContext.RouteData.Values["team"] as string;

            int.TryParse(theHandler.RequestContext.RouteData.Values["level"].ToString(), out level);

            ViewBag.Level = _levelConfigs.Where(c => c.QueueLevel == level).FirstOrDefault();
            ViewBag.QueueMenu = BuildQueueMenu(filterContext, currentTeam, level);
        }

        private void RetrieveCacheValues(ActionExecutingContext filterContext, IDependencyResolver container, DashProfile myProfile)
        {
            _groups = CacheHelper.GetGroups(filterContext.HttpContext, myProfile.Id, container);
            _teams = CacheHelper.GetTeams(filterContext.HttpContext, myProfile.Id, container);
            _pullQueues = CacheHelper.GetQueues(filterContext.HttpContext, container);
	        _scoreCards = _scoreCardApi.GetAllAsync().Result.ScoreCardDtos;
			_reviewItems = CacheHelper.GetOpenReviewable(filterContext.HttpContext, container);
            _levelConfigs = CacheHelper.GetLevelConfigs(filterContext.HttpContext, container);
        }

        private List<QueueMenuItem> BuildQueueMenu(ActionExecutingContext filterContext, string team, int currLevel)
        {
            var ret = new List<QueueMenuItem>();
            //Get the queue we are in
            var q = _pullQueues
                .Where(pq => pq.QueueName == ViewBag.QueueName)
                .First();

            //Get the items this user can see
            var qItems = q.MyPullQueueItems
                .Where(i => i.MinLevelOfConcern <= _loc)
                .OrderBy(i => i.MinLevelOfConcern);

            //Get the list of actions defined by the site for this queue
            var actions = _levelConfigs
                .Where(l => l.QueueId == q.Id)
                .OrderBy(l => l.QueueLevel)
                .Select(l => l.ActionName)
                .Distinct()
                .ToList();

            //Each action is a header with the levels as children
            foreach (var act in actions)
            {
                var children = new List<QueueMenuItem>();
                var totalCount = 0;
                var count = 0;

                //loop through the levels that are assigned this action
                foreach (var workLvl in _levelConfigs
                    .Where(c => c.ActionName.ToLower() == act.ToLower())
                    .ToList())
                {
                    //compile the list of child menu entries
                    count = qItems.Where(qi => qi.MinLevelOfConcern == workLvl.QueueLevel).Count();

                    if (count > 0)
                    {
                        totalCount += count;

                        children.Add(new QueueMenuItem()
                        {
                            Action = act,
                            Active = (workLvl.QueueLevel == currLevel),
                            Children = null,
                            Count = count,
                            Level = workLvl.QueueLevel,
                            Name = workLvl.MenuName,
                            Order = workLvl.QueueLevel
                        });
                    }
                }//End foreach

                //Create the parent entry and add it to ret val

                var firstChild = children.OrderBy(c => c.Order).FirstOrDefault();

                var order = (firstChild == null) ? 99999 : firstChild.Order;

                ret.Add(new QueueMenuItem() { 
                    Action = "",
                    Active = children.Where(c => c.Active).Any(),
                    Children = children.OrderBy(c => c.Order).ToList(),
                    Count = totalCount,
                    Level = 0,
                    Name = act,
                    Order = order
                });
            }

            return ret.OrderBy(r => r.Order).ToList();
        }

    }
}
