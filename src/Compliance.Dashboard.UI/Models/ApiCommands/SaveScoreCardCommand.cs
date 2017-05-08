using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compliance.ScoreCards.Api.Dto.Models.V1;

namespace Compliance.Dashboard.UI.Models.ApiCommands
{
    public class SaveScoreCardCommand
    {
        public ScoreCardResultDto MyScoreCardResult { get; set; }
        public Guid MyQueueItemId { get; set; }
        public Guid AuditEntityId { get; set; }
    }
}