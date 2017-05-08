using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.ScoreCards.Domain.ValueTypes
{
    public class AnswerResponse : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyScoreCardResultId { get; set; }
        public Guid MyAnswerId { get; set; }
        public string Comment { get; set; }
        public DateTime UtcCreated { get; set; }

        public ScoreCardResult MyScoreCardResult { get; set; }
        public Answer MyAnswer { get; set; }
    }
}
