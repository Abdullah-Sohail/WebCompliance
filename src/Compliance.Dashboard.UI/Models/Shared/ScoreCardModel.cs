using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.WorkItems.Domain;

namespace Compliance.Dashboard.UI.Models.Shared
{
    public class ScoreCardModel
    {
        public WorkItem MyWorkItem { get; set; }
        public ICollection<ScoreCardDto> ScoreCards { get; set; }
        public ICollection<ScoreCardResultDto> ScoreCardResults { get; set; }
        public ICollection<ScoreCardReviewDto> ScoreCardReviews { get; set; }
    }
}