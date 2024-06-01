using Enities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountryService : ICountriesService
	{
		private readonly PersonsDbContext dbContext;

		//
		//List<Country> _countries;
		public CountryService(PersonsDbContext dbContext /*bool initialize = true*/)
		{
			//_countries = new List<Country>();


			//if (initialize)
			//{
			//	for (int i = 1; i < 10; i++)
			//	{
			//		var z = AddCountry(new()
			//		{ CountryName = $"Country Number = {i}" });
			//	}
		
			//}

			this.dbContext = dbContext;
		}
		public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
		{

			if (countryAddRequest is null)
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

			if (dbContext.Countries.Count(c => c.Name == countryAddRequest.CountryName) > 0)
			{
				throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
			}

			dbContext.Countries.Add(country);
			dbContext.SaveChanges();

			return country.ToCountryResponse();
		}

		public List<CountryResponse> GetAllCountrise()
		{
			return dbContext.Countries.Select(country => country.ToCountryResponse()).ToList();

		}

		public CountryResponse? GetCountryById(Guid? countryId)
		{
			if(countryId == null)
				return null;


			var result = dbContext.Countries.FirstOrDefault(c => c.CountryId == countryId);

			return result?.ToCountryResponse() ?? null;
		}

	}
}
