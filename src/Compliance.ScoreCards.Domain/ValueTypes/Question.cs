using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.ScoreCards.Domain.ValueTypes
{
    public class Question : ICanGenericRepo
    {
        public Guid Id { get; set; }
        public Guid MyAssertionId { get; set; }
        public Guid? MyParentAnswerId { get; set; }
        //public Guid? MyNaAnswerId { get; set; }
        public string Query { get; set; }
        public string HelpCopy { get; set; }
        public decimal Weight { get; set; }
        public int Order { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public Answer MyParentAnswer { get; set; }
        //public Answer MyNaAnswer { get; set; }
        public Assertion MyAssertion { get; set; }
    }
}
