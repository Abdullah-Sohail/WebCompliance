using System;
using System.Collections.Generic;
//using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.Domain.ValueTypes;
using Compliance.Queuing.Domain;
using Compliance.Queuing.Domain.Service;
using Compliance.Queuing.Domain.ValueTypes;
using Compliance.ScoreCards.Api;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Api.Dto.Packages.V1;
using StructureMap;

namespace Compliance.Dashboard.UI
{
	public static class CacheHelper
	{
		//public static DashProfile GetProfile(HttpContextBase context, IDependencyResolver container)
		//{
		//    //Check session for profile
		//    var ret = context.Session["DashProfile"] as DashProfile;

		//    if (ret != null) return ret;

		//    var pService = container.GetService<IDashProfileService>();

		//    ret = pService.GetByADUser(context.User.Identity.Name);

		//    //Create profile if one doesn't exist
		//    if (ret == null)
		//    {
		//        ret = DashProfile.Create(User.Identity.Name);

		//        using (var ctx = new PrincipalContext(ContextType.Machine))
		//        {
		//            using (var user = UserPrincipal.FindByIdentity(ctx, ret.ADUsername))
		//            {
		//                ret.Name = user.DisplayName;
		//                ret.MyEmail = new EmailAddress(user.EmailAddress);
		//            }
		//        }

		//        pService.CreateProfile(ret);
		//    }

		//    context.Session["DashProfile"] = ret;

		//    return ret;
		//}

		public static DashProfile GetProfileByUserId(string userId, IDependencyResolver container)
		{
			var pService = container.GetService<IDashProfileService>();
			var profile = pService.GetByUserId(new Guid(userId));

			return profile;
		}


		public static ICollection<Team> GetTeams(HttpContextBase context, Guid profileId, IDependencyResolver container)
		{
			var ret = context.Session["Teams"] as ICollection<Team>;

			if (ret != null) return ret;

			return container.GetService<ITeamService>().GetByDashProfileId(profileId);

		}

		public static ICollection<Group> GetGroups(HttpContextBase context, Guid profileId, IDependencyResolver container)
		{
			//Check session for profile
			var ret = context.Session["Groups"] as ICollection<Group>;

			if (ret != null) return ret;

			return container.GetService<IGroupService>().GetByDashProfileId(profileId);
		}

		public static ICollection<PullQueue> GetQueues(HttpContextBase context, IDependencyResolver container)
		{
			ICollection<PullQueue> pullQueues =
				context.Application["PullQueues"] as ICollection<PullQueue>;

			if (pullQueues == null)
			{
				context.Application["NextRefresh"] = DateTime.UtcNow.AddMinutes(30);

				GetQueuesFromService(context, container.GetService<IPullQueueService>()).Wait();

				pullQueues = context.Application["PullQueues"] as ICollection<PullQueue>;

			}
			else if ((DateTime)context.Application["NextRefresh"] < DateTime.UtcNow)
			{
				context.Application["NextRefresh"] = DateTime.UtcNow.AddMinutes(29);

				GetQueuesFromService(context, container.GetService<IPullQueueService>());
			}

			return pullQueues;
		}

		public static ICollection<QueueLevelConfig> GetLevelConfigs(HttpContextBase context, IDependencyResolver container)
		{
			ICollection<QueueLevelConfig> levelConfigs =
				context.Application["QueueLevelConfigs"] as ICollection<QueueLevelConfig>;

			if (levelConfigs == null
				|| (DateTime)context.Application["NextLevelRefresh"] < DateTime.UtcNow)
			{
				var qlcService = container.GetService<IQueueLevelConfigService>();

				context.Application["NextLevelRefresh"] = DateTime.UtcNow.AddMinutes(32);

				levelConfigs = qlcService.GetAll();

				context.Application["QueueLevelConfigs"] = levelConfigs;
			}

			return levelConfigs;
		}

		public static ICollection<ScoreCardDto> GetScoreCards(HttpContext context)
		{
			var api = new ScoreCardApi("", "");
			var scoreCards =
				context.Application["ScoreCards"] as ICollection<ScoreCardDto>;

			if (scoreCards == null
				|| (DateTime)context.Application["NextScoreCardRefresh"] < DateTime.UtcNow)
			{
				context.Application["NextScoreCardRefresh"] = DateTime.UtcNow.AddMinutes(28);

				scoreCards = new List<ScoreCardDto>();
				scoreCards = api.GetAllAsync().Result.ScoreCardDtos;

				context.Application["ScoreCards"] = scoreCards;
			}


			return scoreCards;

		}

		public static ICollection<PullQueueItem> GetOpenReviewable(HttpContextBase context, IDependencyResolver container)
		{
			var ret = context.Application["OpenReviewableItems"] as ICollection<PullQueueItem>;

			if (ret == null)
			{
				context.Application["NextReviewRefresh"] = DateTime.UtcNow.AddMinutes(11);

				GetOpenReviewableFromService(context, GetQueues(context, container), container.GetService<IPullQueueService>())
					.Wait();

				ret = context.Application["OpenReviewableItems"] as ICollection<PullQueueItem>;
			}
			else if ((DateTime)context.Application["NextReviewRefresh"] < DateTime.UtcNow)
			{
				context.Application["NextReviewRefresh"] = DateTime.UtcNow.AddMinutes(11);

				GetOpenReviewableFromService(context, GetQueues(context, container), container.GetService<IPullQueueService>());
			}

			return ret;
		}

		private static Task GetQueuesFromService(HttpContextBase context, IPullQueueService qService)
		{
			return Task.Factory.StartNew(() =>
			{
				var pullQueues = qService.GetActive();

				foreach (var q in pullQueues)
					q.MyPullQueueItems = qService.GetOpenItems(q.Id);

				context.Application["PullQueues"] = pullQueues;
			});
		}

		private static Task GetOpenReviewableFromService(HttpContextBase context, ICollection<PullQueue> queues, IPullQueueService qService)
		{
			return Task.Factory.StartNew(() =>
			{
				var items = new List<PullQueueItem>();

				foreach (var q in queues)
					items.AddRange(qService.GetOpenItemsHavingResults(q.Id));

				context.Application["OpenReviewableItems"] = items;
			});
		}
	}

	public class CachedScoreCardApi : IScoreCardApi
	{
		private readonly IScoreCardApi _innerScoreCardApi;
		private readonly MemoryCache _memoryCache;

		public CachedScoreCardApi(IScoreCardApi innerScoreCardApi, MemoryCache memoryCache)
		{
			_innerScoreCardApi = innerScoreCardApi;
			_memoryCache = memoryCache;
		}

		public async Task<ScoreCard_GetAll_Result> GetAllAsync()
		{
			if (_memoryCache.Contains("CachedScoreCardApi.GetAllAsync"))
			{
				return (ScoreCard_GetAll_Result)_memoryCache.Get("CachedScoreCardApi.GetAllAsync");
			}


			ScoreCard_GetAll_Result ret = await _innerScoreCardApi.GetAllAsync();
			_memoryCache.Add("CachedScoreCardApi.GetAllAsync", ret, new CacheItemPolicy()
			{
				SlidingExpiration = new TimeSpan(1, 0, 0)
			});
			return ret;
		}
	}

	public class MockScoreCardApi : IScoreCardApi
	{
		public Task<ScoreCard_GetAll_Result> GetAllAsync()
		{
			return Task.FromResult(new ScoreCard_GetAll_Result());
		}
	}

}