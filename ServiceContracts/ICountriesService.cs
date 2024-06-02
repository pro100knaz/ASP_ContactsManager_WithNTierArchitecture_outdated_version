using Microsoft.AspNetCore.Http;
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
		/// 
		Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

		Task<List<CountryResponse>> GetAllCountrise();

		Task<CountryResponse?> GetCountryById(Guid? countryId);

		Task<int> UploadCountriesFromExcelFile(IFormFile formFile);

	}
}
