using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.ScoreCards.Domain.ValueTypes;

namespace Compliance.ScoreCards.Implementation.Ef.Maps
{
    public class AnswerMap : EntityTypeConfiguration<Answer>
    {
        public AnswerMap()
        {
            this.HasMany(a => a.ChildQuestions)
                .WithOptional(q => q.MyParentAnswer)
                .HasForeignKey(q => q.MyParentAnswerId);

        }
    }
}
