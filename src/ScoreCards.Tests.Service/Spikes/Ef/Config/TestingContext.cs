using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.ScoreCards.Implementation.Ef;

namespace ScoreCards.Tests.Service.Spikes.Ef.Config
{
    public class TestingContext : IMakeDbContext<ScoreCardsContext>
    {
        private const string _connString =
             "Data Source=localhost\\sqlexpress;Initial Catalog=ComplianceScoreCards;Integrated Security=SSPI;";

        public System.Data.Entity.DbContext GetContext()
        {
            return new ScoreCardsContext(_connString);
        }
    }
}
