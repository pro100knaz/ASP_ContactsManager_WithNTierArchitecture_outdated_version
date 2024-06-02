using Enities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountryService : ICountriesService
	{
		private readonly ICountriesRepository countriesRepository;

		public CountryService(ICountriesRepository countriesRepository)
		{
			this.countriesRepository = countriesRepository;
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

			if (await countriesRepository.GetCountryByCountryName(country.Name) is not null)
			{
				throw new ArgumentException(nameof(countryAddRequest.CountryName));
			}

			await countriesRepository.AddCountry(country);
			return country.ToCountryResponse();
		}

		public async Task<List<CountryResponse>> GetAllCountrise()
		{
			return (await countriesRepository.GetCountries()).Select(country => country.ToCountryResponse()).ToList();
		}

		public async Task<CountryResponse?> GetCountryById(Guid? countryId)
		{
			if (countryId == null)
				return null;

			var result = await countriesRepository.GetCountryByCountryId(countryId.Value);

			return result?.ToCountryResponse() ?? null;
		}

		public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
		{
			MemoryStream memoryStream = new MemoryStream();
			await formFile.CopyToAsync(memoryStream);

			int countriesInserted = 0;

			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];
				int rowCount = excelWorksheet.Dimension.Rows;
				for (int row = 2; row < rowCount; row++)
				{
					string? cellValue = Convert.ToString(excelWorksheet.Cells[row, 1].Value);

					if (!string.IsNullOrEmpty(cellValue))
					{
						string countryName = cellValue;

						if (await countriesRepository.GetCountryByCountryName(countryName) is null)
						{
							await AddCountry(new CountryAddRequest() { CountryName = countryName });
							countriesInserted++;
   						}
					}
				}
			}

			return countriesInserted;
		}
	}
}
