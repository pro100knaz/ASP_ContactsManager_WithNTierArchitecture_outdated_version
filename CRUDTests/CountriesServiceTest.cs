
using Enities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{


	public class CountriesServiceTest
	{
		private readonly ICountriesService _countriesService;

		public CountriesServiceTest()
		{

			//generate entities
			var countriesList = new List<Country>() { }; //can add initial data

			//Create DbContext mock:
			var mockDb = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder().Options);

			//Setup DbSet or DbQuery property:

			mockDb.CreateDbSetMock(temp => temp.Countries, countriesList);

			//var dbContext = mockDb.Object;

			_countriesService = new CountryService(null);
		}
		// null - throw ARGUM

		#region  AddCountry


		[Fact]
		public async Task AddCountry_NullCountry()
		{
			//Arrange
			CountryAddRequest? request = null;

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			//Act
			await _countriesService.AddCountry(request));

		}

		//Name is null Argum except
		[Fact]
		public async Task AddCountry_CountryNameIsNull()
		{
			//Arrange
			CountryAddRequest? request = new() { CountryName = null };

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			//Act
			await _countriesService.AddCountry(request));

		}

		//When the Country is duplicate - throe exception
		[Fact]
		public async Task AddCountry_CountryNameIsDuplicate()
		{
			//Arrange
			CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "null" };
			CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "null" };

			//Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				//Act
				await _countriesService.AddCountry(request2);
				await _countriesService.AddCountry(request1);

			});

		}

		//proper name - supply
		[Fact]
		public async Task AddCountry_ProperCountryDetails()
		{
			//Arrange
			CountryAddRequest? request = new() { CountryName = "japan" };
			//Act
			CountryResponse response = await _countriesService.AddCountry(request);

			var listResponse = await _countriesService.GetAllCountrise();

			//Assert
			Assert.True(response.CountryId != Guid.Empty);

			Assert.Equal(new List<CountryResponse> { response }, listResponse);
		}

		#endregion

		#region  GetAllCountries

		[Fact]

		public async Task GetAllCountries_EmptyList()
		{
			//Acts
			List<CountryResponse> actual_country_from_response_list =
				await _countriesService.GetAllCountrise();

			//Asserts
			Assert.Empty(actual_country_from_response_list);

		}
		[Fact]
		public async Task GetAllCountries_AddFewCountries()
		{
			//Acts
			List<CountryAddRequest> country_request_list =
				new()
				{
					new CountryAddRequest(){ CountryName = "wdaw123d"},
					new CountryAddRequest(){ CountryName = "w2dawd"},
					new CountryAddRequest(){ CountryName = "34wdawd"},
					new CountryAddRequest(){ CountryName = "w24dawd"}
				};


			List<CountryResponse> response_list = new();

			foreach (var c in country_request_list)
			{
				response_list.Add(await _countriesService.AddCountry(c));
			}

			List<CountryResponse> ListResponse = await _countriesService.GetAllCountrise();


			foreach (CountryResponse response in ListResponse)
			{
				Assert.Contains(response, response_list);
			}

		}

		#endregion

		#region GetCountryById

		[Fact]
		public async Task GetCountryById_NullParam()
		{
			//Arrange
			Guid countryId = Guid.Empty;

			//Act
			CountryResponse? x = await _countriesService.GetCountryById(countryId);

			//Assert
			Assert.Null(x);
		}
		[Fact]
		public async Task GetCountryById_ProperParam()
		{
			//Arrange
			//Guid countryId = Guid.NewGuid();

			CountryAddRequest? requestCountry = new CountryAddRequest()
			{
				CountryName = "USA"
			};

			var responseAdd = await _countriesService.AddCountry(requestCountry);

			//Act
			CountryResponse? responseGet = await _countriesService.GetCountryById(responseAdd.CountryId);

			//Assert
			Assert.Equal(responseAdd, responseGet);
		}

		#endregion


	}
}
