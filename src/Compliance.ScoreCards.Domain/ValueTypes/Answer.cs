using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.ScoreCards.Domain.ValueTypes
{
    public class Answer : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyQuestionId { get; set; }
        public string Label { get; set; }
        public int Percentage { get; set; }
        public int Order { get; set; }
        public Question MyQuestion { get; set; }

        public ICollection<Question> ChildQuestions { get; set; }
    }
}
