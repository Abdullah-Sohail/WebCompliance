using System;
using Microsoft.AspNetCore.Mvc;

namespace Compiance.Audio.Host.AspNetCore.Models
{
	public partial class LatitudeController : Controller
    {
		public class AccountNote
        {
            public int AccountId { get; set; }
            public string Action { get; set; }
            public string Result { get; set; }
            public string Comment { get; set; }
            public string Username { get; set; }
            public string Alias { get; set; }
            public string RealName { get; set; }
            public DateTime Created { get; set; }
        }
    }
}
