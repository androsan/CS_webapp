using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Diagnostics;

namespace CS_webapp.Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }

		[DisplayName("Uporabni�ko ime")]
		//[Required]
		public string Name { get; set; }

		[DisplayName("Geslo")]
		//[Required]
		//[Range(1,int.MaxValue,ErrorMessage ="�tevilka naro�ila mora biti ve�ja od 0!")]
		public string DisplayOrder { get; set; }

		[DisplayName("Datum registracije")]
		//[Required]
		public string InitDate { get; set; }

		[DisplayName("Datum spremembe gesla")]
		//[Required]
		public string ResetDate { get; set; }

		[DisplayName("Datum zadnjega logiranja")]
		//[Required]
		public string LastDate { get; set; }

	}

	public class ResetirajGeslo
    {
		//[Required]
		[DisplayName("Novo geslo")]
		public string NewOrderFirst { get; set; }

		//[Required]
		[DisplayName("Ponovi geslo")]
		public string NewOrderSecond { get; set; }	
	}

}