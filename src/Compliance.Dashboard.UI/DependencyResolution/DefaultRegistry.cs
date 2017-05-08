// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Caching;
using Compliance.ScoreCards.Api;

namespace Compliance.Dashboard.UI.DependencyResolution {
    using StructureMap;
    using Compliance.Common.GenericRepo.Implementation;
    using Compliance.Dashboard.Implementation.Ef;
    using Compliance.Common.GenericRepo.Interfaces;
    using Compliance.Dashboard.Domain;
    using Compliance.Audio.Domain;
    using Compliance.Dashboard.Domain.Service;
    using Compliance.Dashboard.Implementation.Ef.Services;
    using Compliance.Audio.Implementation.Ef.Persistence.Data;
    using Compliance.Queuing.Implementation.Ef;
    using Compliance.Queuing.Domain;
    using Compliance.Queuing.Domain.ValueTypes;
    using Compliance.Queuing.Domain.Service;
    using Compliance.WorkItems.Implementation.Ef;
    using Compliance.Queuing.Implementation.Ef.Services;
    using Compliance.WorkItems.Domain;
    using Compliance.WorkItems.Domain.Service;
    using Compliance.WorkItems.Implementation.Ef.Services;
    using Compliance.Audio.Implementation.Ef.Services;
    using Compliance.Audio.Domain.Service;
    using Compliance.Dashboard.UI.Mocks;
    using Domain.ValueType;
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
			//Dashboard Domain
			For<IMakeDbContext<DashboardContext>>().Use<DashContextFromConfig>();

			For<IGenericRepo<Group, DashboardContext>>().Use<GenericRepo<Group, DashboardContext>>();
			For<IGenericRepo<Team, DashboardContext>>().Use<GenericRepo<Team, DashboardContext>>();
            For<IGenericRepo<QueueAssignment, DashboardContext>>().Use<GenericRepo<QueueAssignment, DashboardContext>>();

            For<IGenericRepo<DashProfile, DashboardContext>>().Use<GenericRepo<DashProfile, DashboardContext>>();
			For<IGenericRepo<QueueLevelConfig, DashboardContext>>().Use<GenericRepo<QueueLevelConfig, DashboardContext>>();
			For<IGenericRepo<Recording, RecordingContext>>().Use<GenericRepo<Recording, RecordingContext>>();
			For<IGenericRepo<AgentLogin, RecordingContext>>().Use<GenericRepo<AgentLogin, RecordingContext>>();

			For<IDashProfileService>().Use<DashProfileService>();
			For<IGroupService>().Use<GroupService>();
			For<ITeamService>().Use<TeamService>();
			For<IQueueLevelConfigService>().Use<QueueLevelConfigService>();

			//Queueing Domain
			For<IMakeDbContext<QueuingContext>>().Use<QueueContextFromConfig>();

			For<IGenericRepo<PullQueue, QueuingContext>>().Use<GenericRepo<PullQueue, QueuingContext>>();
			For<IGenericRepo<PullQueueItem, QueuingContext>>().Use<GenericRepo<PullQueueItem, QueuingContext>>();
			For<IGenericRepo<PullQueueItemAction, QueuingContext>>().Use<GenericRepo<PullQueueItemAction, QueuingContext>>();

			For<IPullQueueService>().Use<PullQueueService>();

			//WorkItem Domain
			For<IMakeDbContext<WorkItemsContext>>().Use<WorkItemsContextFromConfig>();

			For<IGenericRepo<RecordingItem, WorkItemsContext>>().Use<GenericRepo<RecordingItem, WorkItemsContext>>();

			For<IRecordingItemService>().Use<RecordingItemService>();

			For<IRecordingService>().Use<RecordingService>();

			For<IAgentLoginService>().Use<AgentLoginService>();

			For<IMakeDbContext<RecordingContext>>().Use<RecordingContextFromConfig>();

			For<IApiCalls>().Use(x => new MockCallInfoApi(System.Configuration.ConfigurationManager.AppSettings["QueueService"]));
	        For<IScoreCardApi>()
		        .Use(x => new CachedScoreCardApi(new MockScoreCardApi(), new System.Runtime.Caching.MemoryCache("Main", null)));
        }

        #endregion
    }
}