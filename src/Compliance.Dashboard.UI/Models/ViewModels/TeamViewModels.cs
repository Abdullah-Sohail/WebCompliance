using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Compliance.Dashboard.Domain.ValueType;

namespace Compliance.Dashboard.UI.Models
{
    public class TeamViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Team Name")]
        public string TeamName { get; set; }
        public int TeamMembers { get; set; }
        public int TeamQueues { get; set; }
        public List<Guid> SelectedQueues { get; set; }
        public DateTime UtcCreated { get; set; }
        public bool IsActive { get; set; }
    }
}