using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TrackViewer.Models
{
	public class RegisterViewModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		[Display(Name="Re-enter Password")]
		public string Password2 { get; set; }
	}
}