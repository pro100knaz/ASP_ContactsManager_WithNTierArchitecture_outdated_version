using System;
using Enities;

namespace ServiceContracts.DTO
{
	public class CountryResponse
	{
		public Guid CountryId { get; set; }
		public string? CountryName { get; set; }


		public override bool Equals(object? obj)
		{
			if (obj is CountryResponse country)
			{
				return this.CountryId == country.CountryId 
					&& this.CountryName == country.CountryName;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.CountryId.GetHashCode();
			//throw new NotImplementedException();
		}
	}

	public static class CountryExtensions
	{
		public static CountryResponse ToCountryResponse(this Country country)
		{
			return new CountryResponse()
			{
				CountryId = country.CountryId,
				CountryName = country.Name
			};
		}
	}

}
