using Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
	public class CountryAddRequest
	{
		public string? CountryName { get; set; }

		public Country ToCountry()
		{
			return new Country()
			{
				Name = CountryName,
			};


		}
	}
}
