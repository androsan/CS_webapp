using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Diagnostics;

namespace CS_webapp.Models
{
	public class BigModel
	{

		public Category Category { get; set; }
		public LoginModel LoginModel { get; set; }
		public ResetirajGeslo ResetirajGeslo { get; set; }
		public Podatki Podatki { get; set; }

	}

}