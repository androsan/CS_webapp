using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS_webapp.Models
{
	public class Category
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		[DisplayName("Uporabniško ime")]
		[Required]
		public string Name { get; set; }

		[DisplayName("Geslo")]
		[Required]
		//[Range(1,int.MaxValue,ErrorMessage ="Številka naroèila mora biti veèja od 0!")]
		public string DisplayOrder { get; set; }

		[DisplayName("Datum registracije")]
		public string InitDate { get; set; }

		[DisplayName("Datum spremembe gesla")]
		public string ResetDate { get; set; }

		[DisplayName("Datum zadnjega logiranja")]
		public string LastDate { get; set; }

		// THIS WAS WRONG, FOREIGN KEY SHOULD NOT BE DECLARED IN PARENT TABLE!!! Wrong in YouTube tutorial!
		/*
		[ForeignKey("Name")]
		public Podatki Podatki { get; set; }
		*/

		//public string ajax_variable { get; set; }


	}

	public class ResetirajGeslo
    {
		[Required]
		[DisplayName("Novo geslo")]
		public string NewOrderFirst { get; set; }

		[Required]
		[DisplayName("Ponovi geslo")]
		public string NewOrderSecond { get; set; }	
	}

}