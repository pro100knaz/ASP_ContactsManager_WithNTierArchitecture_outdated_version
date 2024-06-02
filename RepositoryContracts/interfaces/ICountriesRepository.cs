using Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.interfaces
{
	public interface ICountriesRepository
	{
		Task<Country> AddCountry(Country country); 

		Task<List<Country>> GetCountries();

		Task<Country?> GetCountryByCountryId(Guid countryId);

		Task<Country?> GetCountryByCountryName(string CountryName);

	}
}
