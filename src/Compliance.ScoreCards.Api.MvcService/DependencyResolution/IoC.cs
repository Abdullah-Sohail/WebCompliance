// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
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


using Compliance.Common.GenericRepo.Implementation;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.Services;
using Compliance.ScoreCards.Implementation.Ef;
using Compliance.ScoreCards.Implementation.Ef.Data;
using Compliance.ScoreCards.Implementation.Ef.Services;
using StructureMap;
namespace Compliance.ScoreCards.Api.MvcService.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.For<IMakeDbContext<ScoreCardsContext>>().Use<ScoreCardContextFromConfig>();
                            x.For<IGenericRepo<ScoreCard, ScoreCardsContext>>().Use<GenericRepo<ScoreCard, ScoreCardsContext>>();
                            x.For<IGenericRepo<ScoreCardResult, ScoreCardsContext>>().Use<GenericRepo<ScoreCardResult, ScoreCardsContext>>();
                            x.For<IGenericRepo<ScoreCardReview, ScoreCardsContext>>().Use<GenericRepo<ScoreCardReview, ScoreCardsContext>>();
                            x.For<IScoreCardService>().Use<ScoreCardService>();
                        });
            return ObjectFactory.Container;
        }
    }
}