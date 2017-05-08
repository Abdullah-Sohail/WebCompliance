using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Api.DtoAdapter.V1
{
    public static class ScoreCardAdapter
    {
        public static ScoreCard MakeDomain(ScoreCardDto dto)
        {
            var ret = new ScoreCard()
            {
                Id = dto.Id,
                MyLastVersionId = dto.MyLastVersionId,
                Title = dto.Title,
                UtcCreated = dto.UtcCreated,
                Version = dto.Version,
                Assertions = new List<Assertion>()
            };

            foreach (var a in dto.Assertions)
            {
                var theA = new Assertion()
                {
                    Id = a.Id,
                    EscalationLevelOfConcern = a.EscalationLevelOfConcern,
                    MyScoreCardId = a.MyScoreCardId,
                    Order = a.Order,
                    PassThreshold = a.PassThreshold,
                    Questions = new List<Question>(),
                    Statement = a.Statement
                };

                foreach (var q in a.Questions)
                {
                    var theQ = new Question()
                    {
                        Id = q.Id,
                        HelpCopy = q.HelpCopy,
                        MyAssertionId = q.MyAssertionId,
                        MyParentAnswerId = q.MyParentAnswerId,
                        Order = q.Order,
                        Query = q.Query,
                        Weight = q.Weight,
                        Answers = new List<Answer>()
                    };

                    foreach (var ans in q.Answers)
                        theQ.Answers.Add(new Answer()
                        {
                            Id = ans.Id,
                            Label = ans.Label,
                            MyQuestionId = ans.MyQuestionId,
                            Order = ans.Order,
                            Percentage = ans.Percentage
                        });

                    theA.Questions.Add(theQ);
                }

                ret.Assertions.Add(theA);
            }

            return ret;
        }
        public static ScoreCardDto MakeDto(ScoreCard domain)
        {
            var ret = new ScoreCardDto()
            {
                Id = domain.Id,
                MyLastVersionId = domain.MyLastVersionId,
                Title = domain.Title,
                UtcCreated = domain.UtcCreated,
                Version = domain.Version,
                Assertions = new List<AssertionDto>()
            };

            foreach (var a in domain.Assertions)
            {
                var theA = new AssertionDto()
                {
                    Id = a.Id,
                    EscalationLevelOfConcern = a.EscalationLevelOfConcern,
                    MyScoreCardId = a.MyScoreCardId,
                    Order = a.Order,
                    PassThreshold = a.PassThreshold,
                    Questions = new List<QuestionDto>(),
                    Statement = a.Statement
                };

                foreach (var q in a.Questions)
                {
                    var theQ = new QuestionDto()
                    {
                        Id = q.Id,
                        HelpCopy = q.HelpCopy,
                        MyAssertionId = q.MyAssertionId,
                        MyParentAnswerId = q.MyParentAnswerId,
                        Order = q.Order,
                        Query = q.Query,
                        Weight = q.Weight,
                        Answers = new List<AnswerDto>()
                    };

                    foreach (var ans in q.Answers)
                        theQ.Answers.Add(new AnswerDto()
                        {
                            Id = ans.Id,
                            Label = ans.Label,
                            MyQuestionId = ans.MyQuestionId,
                            Order = ans.Order,
                            Percentage = ans.Percentage
                        });

                    theA.Questions.Add(theQ);
                }

                ret.Assertions.Add(theA);
            }

            return ret;
        }
    }
}
