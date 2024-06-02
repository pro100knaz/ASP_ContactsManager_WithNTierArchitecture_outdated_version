using Enities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
		public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
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

			if (await dbContext.Countries.CountAsync(c => c.Name == countryAddRequest.CountryName) > 0)
			{
				throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
			}

			await dbContext.Countries.AddAsync(country);
			await dbContext.SaveChangesAsync();

			return country.ToCountryResponse();
		}

		public async Task<List<CountryResponse>> GetAllCountrise()
		{
			return await dbContext.Countries.Select(country => country.ToCountryResponse()).ToListAsync();

		}

		public async Task<CountryResponse?> GetCountryById(Guid? countryId)
		{
			if(countryId == null)
				return null;


			var result = await dbContext.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);

			return result?.ToCountryResponse() ?? null;
		}

		public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
		{
			MemoryStream memoryStream = new MemoryStream();
			await formFile.CopyToAsync(memoryStream);

			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];
				int rowCount = excelWorksheet.Dimension.Rows;
			}
		}
	}
}
