using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System;
using System.Diagnostics;

namespace CS_webapp.Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[DisplayName("Display Order")]
		[Required]
		[Range(1,int.MaxValue,ErrorMessage ="Številka naroèila mora biti veèja od 0!")]
		public int DisplayOrder { get; set; }

		//public string ModDate { get; set; }

		//public string ModDate = DateTime.Now.ToString("HH:mm:ss tt");
		//Debug.WriteLine(object localDate);

	}
}