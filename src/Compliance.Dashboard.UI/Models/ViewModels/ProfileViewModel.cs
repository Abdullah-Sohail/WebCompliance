using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Compliance.Dashboard.UI.Models
{
    public class ProfileViewModel
    {

        public Guid UserId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        [Display(Name = "Cell Number")]
        public string CellNumber { get; set; }
        public DateTime UtcCreated { get; set; }
        [Display(Name = "Select Role")]
        public string SelectedRole { get; set; }
    }
}