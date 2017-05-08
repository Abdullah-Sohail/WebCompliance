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
    public static class ScoreCardResultAdapter
    {
        public static ScoreCardResult MakeDomain(ScoreCardResultDto dto)
        {
            var ret = new ScoreCardResult()
            {
                AuditEntityId = dto.AuditEntityId,
                AuditEntityType = dto.AuditEntityType,
                Id = dto.Id,
                MyScoreCardId = dto.MyScoreCardId,
                UtcCreated = dto.UtcCreated,
                WorkItemId = dto.WorkItemId,
                WorkItemType = dto.WorkItemType,
                Answers = new List<AnswerResponse>()
            };

            foreach (var a in dto.Answers)
            {
                ret.Answers.Add(new AnswerResponse() { 
                    Id = a.Id,
                    Comment = a.Comment,
                    MyAnswerId = a.MyAnswerId,
                    MyScoreCardResultId = a.MyScoreCardResultId,
                    UtcCreated = a.UtcCreated
                });
            }

            return ret;
        }

        public static ScoreCardResultDto MakeDto(ScoreCardResult domain)
        {
            var ret = new ScoreCardResultDto()
            {
                AuditEntityId = domain.AuditEntityId,
                AuditEntityType = domain.AuditEntityType,
                Id = domain.Id,
                MyScoreCardId = domain.MyScoreCardId,
                UtcCreated = domain.UtcCreated,
                WorkItemId = domain.WorkItemId,
                WorkItemType = domain.WorkItemType,
                Answers = new List<AnswerResponseDto>()
            };

            foreach (var a in domain.Answers)
            {
                ret.Answers.Add(new AnswerResponseDto()
                {
                    Id = a.Id,
                    Comment = a.Comment,
                    MyAnswerId = a.MyAnswerId,
                    MyScoreCardResultId = a.MyScoreCardResultId,
                    UtcCreated = a.UtcCreated
                });
            }

            return ret;
        }
    }
}
