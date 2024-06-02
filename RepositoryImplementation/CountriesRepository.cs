using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesImplementation
{
	public class CountriesRepository : ICountryRepository
	{
		private readonly ApplicationDbContext applicationDbContext;

		public CountriesRepository(ApplicationDbContext applicationDbContext)
		{
			this.applicationDbContext = applicationDbContext;
		}

		public async Task<Country> AddCountry(Country country)
		{
			await applicationDbContext.AddAsync(country);
			applicationDbContext.SaveChanges();
			return country;
		}

		public async Task<IEnumerable<Country>> GetCountries()
		{
			return await applicationDbContext.Countries.ToListAsync();
		}

		public async Task<Country?> GetCountryByCountryId(Guid countryId)
		{
			return await applicationDbContext.Countries.FirstOrDefaultAsync(country => country.CountryId == countryId);
		}

		public async Task<Country?> GetCountryByCountryName(string CountryName)
		{
			return await applicationDbContext.Countries.FirstOrDefaultAsync(country => string.Equals(country.Name, CountryName));
		}
	}
}
