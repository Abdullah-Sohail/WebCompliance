using System;
using Microsoft.AspNetCore.Mvc;

namespace Compiance.Audio.Host.AspNetCore.Models
{
	public partial class LatitudeController : Controller
    {
		public class InfoRequest
        {
            public string ForPhone { get; set; }
            public DateTime TheDate { get; set; }

        }
    }
}
