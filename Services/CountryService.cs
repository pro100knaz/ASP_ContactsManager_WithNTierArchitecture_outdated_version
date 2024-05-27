using Enities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountryService : ICountriesService
	{
		List<Country> _countries = new List<Country>();
		public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
		{

			if(countryAddRequest is null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest));
			}


			if (countryAddRequest.CountryName is null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
			}

			
			Country country = new Country();

			country.Name = countryAddRequest.CountryName;

			country.CountryId = Guid.NewGuid();

			if(_countries.Where(c => c.Name == countryAddRequest.CountryName).Count() > 0)
			{
				throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
			}

			_countries.Add(country);

			return country.ToCountryResponse();
		}

		public List<CountryResponse> GetAllCountrise()
		{
			throw new NotImplementedException();
		}
	}
}
