﻿using System.ComponentModel.DataAnnotations;

namespace Enities
{
	/// <summary>
	/// Domain Model for Country
	/// </summary>
	public class Country
	{
		[Key]
		public Guid CountryId { get; set; }

		public string? Name { get; set; }	
	}
}
