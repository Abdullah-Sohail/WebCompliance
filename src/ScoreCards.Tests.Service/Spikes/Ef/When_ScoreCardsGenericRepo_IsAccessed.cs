using System;
using System.Linq;
using Compliance.Common.GenericRepo.Implementation;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.ValueTypes;
using Compliance.ScoreCards.Implementation.Ef;
using Compliance.ScoreCards.Implementation.Ef.Services;
using NUnit.Framework;
using ScoreCards.Tests.Service.Spikes.Ef.Config;

namespace ScoreCards.Tests.Service.Spikes.Ef
{
    [TestFixture]
    public class When_ScoreCardsGenericRepo_IsAccessed
    {
        [Test]
        public void _SeedData_Creates()
        {
            var repo = new GenericRepo<ScoreCard, ScoreCardsContext>(new TestingContext());

            Assert.AreEqual(repo.Query(g => g.Title == "Answering Machine Message").ToList().Count, 1);
        }

        [Test]
        public void _FullScoreCard_Returns()
        {
            var theService = new ScoreCardService(
                new GenericRepo<ScoreCard, ScoreCardsContext>(new TestingContext()),
                new GenericRepo<ScoreCardResult, ScoreCardsContext>(new TestingContext()),
                new GenericRepo<ScoreCardReview, ScoreCardsContext>(new TestingContext()));

            var fullCard = theService.GetById(Guid.Parse("BA0B0472-55B4-427C-9CC1-F254690949D7"));

            Assert.IsNotNull(fullCard.Assertions.ToList()[0].Questions.ToList()[0].Answers.ToList()[0]);
        }
    }
}
