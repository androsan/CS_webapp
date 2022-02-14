using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System;
using System.Diagnostics;

namespace CS_webapp.Models
{
	public class LoginModel
	{
		//[Key]
		//public string Id { get; set; }

		[DisplayName("Uporabniško ime")]
		[Required]
		//[Range(1, int.MaxValue, ErrorMessage = "Neveljavno uporabniško ime!")]
		public string UserName { get; set; }

		[DisplayName("Geslo")]
		[Required]
		//[Range(1,int.MaxValue,ErrorMessage ="Neveljavno geslo!")]
		public string Password { get; set; }

	}
}