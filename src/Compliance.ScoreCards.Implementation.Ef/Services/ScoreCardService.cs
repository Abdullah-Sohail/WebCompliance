using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.Services;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Implementation.Ef.Services
{
    public class ScoreCardService : IScoreCardService
    {
        private IGenericRepo<ScoreCard, ScoreCardsContext> _scoreCardRepo;
        private IGenericRepo<ScoreCardResult, ScoreCardsContext> _scoreCardResultRepo;
        private IGenericRepo<ScoreCardReview, ScoreCardsContext> _scoreCardReviewRepo;
        private string[] _valueIncludes;

        public ScoreCardService(
            IGenericRepo<ScoreCard, ScoreCardsContext> scoreCardRepo,
            IGenericRepo<ScoreCardResult, ScoreCardsContext> scoreCardResultRepo,
            IGenericRepo<ScoreCardReview, ScoreCardsContext> scoreCardReviewRepo)
        {
            _scoreCardRepo = scoreCardRepo;
            _scoreCardResultRepo = scoreCardResultRepo;
            _scoreCardReviewRepo = scoreCardReviewRepo;

            _valueIncludes = new string[] { "Assertions", "Assertions.Questions", "Assertions.Questions.Answers" };
        }

        public ScoreCard GetById(Guid scoreCardId)
        {
            var theCard = _scoreCardRepo.Query(s => s.Id == scoreCardId, _valueIncludes).First();

            return theCard;
        }

        public ICollection<ScoreCard> GetAll()
        {
            return _scoreCardRepo.Query(s => s.Id != null, _valueIncludes).ToList();
        }

        public ScoreCardResult SaveScoreCardResult(ScoreCardResult result)
        {

            //Maintain Id's and Created
            if (result.Id == Guid.Empty)
                result.Id = Guid.NewGuid();

            result.UtcCreated = DateTime.UtcNow;

            foreach (var ans in result.Answers)
            {
                if (ans.Id == Guid.Empty)
                    ans.Id = Guid.NewGuid();

                ans.MyScoreCardResultId = result.Id;
                ans.UtcCreated = DateTime.UtcNow;
            }

            //Save the scorecard result
            _scoreCardResultRepo.Add(result);
            _scoreCardResultRepo.Save();

            return result;
        }

        public ScoreCardReview GenerateReview(ScoreCardResult result)
        {
            var sc = GetById(result.MyScoreCardId);
            var ret = new ScoreCardReview()
            {
                Id = Guid.NewGuid(),
                MyScoreCardResultId = result.Id,
                AssertionReviews = new List<AssertionReview>(),
                UtcCreated = DateTime.UtcNow
            };

            //Each assertion has a pass threshold calculated by the aggregate of 
            //question weight times selected answer's percentage for each question
            foreach (var a in sc.Assertions)
            {
                var pass = a.PassThreshold;
                var score = 0m;

                foreach (var q in a.Questions)
                    foreach (var qa in q.Answers)
                        if (result.Answers.Where(x => x.MyAnswerId == qa.Id).Any())
                            score += q.Weight * (qa.Percentage / 100m);

                ret.AssertionReviews.Add(new AssertionReview()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = a.Id,
                    MyScoreCardReviewId = ret.Id,
                    AuditEntityType = result.AuditEntityType,
                    AuditEntityId = result.AuditEntityId,
                    IsPassed = score >= pass,
                    Passing = pass,
                    Score = score,
                    UpdateNote = "",
                    UtcLastUpdated = DateTime.UtcNow
                });
            }

            _scoreCardReviewRepo.Add(ret);
            _scoreCardReviewRepo.Save();

            return ret;
        }

        public ICollection<ScoreCardResult> GetResultsByWorkItem(Guid workItemId)
        {
            return _scoreCardResultRepo
                .Query(scr => scr.WorkItemId == workItemId, new string[] { "Answers" }).ToList();
        }

        public ICollection<ScoreCardReview> GetReviewsByWorkItem(Guid workItemId)
        {
            return _scoreCardReviewRepo
                .Query(scr => scr.MyScoreCardResult.WorkItemId == workItemId, new string[] { "AssertionReviews" }).ToList();
        }
    }
}
