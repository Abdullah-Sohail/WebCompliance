using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Implementation.Ef.Maps
{
    public class QuestionMap : EntityTypeConfiguration<Question>
    {
        public QuestionMap()
        {
            HasMany(q => q.Answers)
                .WithRequired(a => a.MyQuestion)
                .HasForeignKey(a => a.MyQuestionId);
        }
    }
}
