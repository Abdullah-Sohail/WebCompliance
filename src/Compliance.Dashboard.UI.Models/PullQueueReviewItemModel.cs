using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compliance.Queuing.Domain.ValueTypes;
using Compliance.ScoreCards.Api.Dto.Models.V1;
using Compliance.WorkItems.Domain;

namespace Compliance.Dashboard.UI.Models
{
    public class PullQueueReviewItemModel
    {
        public PullQueueItem MyQueueItem { get; set; }
        public RecordingItem MyRecordingItem { get; set; }
        public ICollection<AccountInfo> RelatedAccounts { get; set; }
        public ICollection<ScoreCardDto> ScoreCards { get; set; }
        public ICollection<ScoreCardResultDto> ScoreCardResults { get; set; }
        public ICollection<ScoreCardReviewDto> ScoreCardReviews { get; set; }
        public DateTime UtcPulled { get; set; }
    }
}