using System.Configuration;
using System.Data.Entity;
using Compliance.Common.GenericRepo.Interfaces;

namespace Compliance.ScoreCards.Implementation.Ef.Data
{
    public class ScoreCardContextFromConfig : IMakeDbContext<ScoreCardsContext>
    {
        public DbContext GetContext()
        {
            return new ScoreCardsContext(ConfigurationManager.ConnectionStrings["ComplianceScoreCardsDb"].ConnectionString);
        }
    }
}
