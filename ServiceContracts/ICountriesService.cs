using ServiceContracts.DTO;

namespace ServiceContracts
{
	public interface ICountriesService
	{
		/// <summary>
		/// Adds a country object to the list of countries
		/// </summary>
		/// <param name="countryAddRequest">Country object to add</param>
		/// <returns>Reeturns the country object after adding it</returns>
		CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

		List<CountryResponse> GetAllCountrise();


		CountryResponse? GetCountryById(Guid? countryId);

	}
}
