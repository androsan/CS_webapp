using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS_webapp.Models
{
	public class Podatki
	{

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		[DisplayName("Uporabnik")]
		public string Name { get; set; }

		[DisplayName("Secret Content")]
		//[Required]
		public string Secret { get; set; }
		
		[ForeignKey("Id")]
		public Category Category { get; set; }

	}


}