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
			_countriesService = new CountryService();
		}
		// null - throw ARGUM

		#region  AddCountry


		[Fact]
		public void AddCountry_NullCountry()
		{
			//Arrange
			CountryAddRequest? request = null;

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			//Act
			_countriesService.AddCountry(request));

		}

		//Name is null Argum except
		[Fact]
		public void AddCountry_CountryNameIsNull()
		{
			//Arrange
			CountryAddRequest? request = new() { CountryName = null };

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			//Act
			_countriesService.AddCountry(request));

		}

		//When the Country is duplicate - throe exception
		[Fact]
		public void AddCountry_CountryNameIsDuplicate()
		{
			//Arrange
			CountryAddRequest? request2 = new() { CountryName = "null" };
			CountryAddRequest? request1 = new() { CountryName = "null" };

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act
				_countriesService.AddCountry(request2);
				_countriesService.AddCountry(request1);
			});

		}

		//proper name - supply
		[Fact]
		public void AddCountry_ProperCountryDetails()
		{
			//Arrange
			CountryAddRequest? request = new() { CountryName = "japan" };
			//Act
			CountryResponse response = _countriesService.AddCountry(request);

			var listResponse = _countriesService.GetAllCountrise();

			//Assert
			Assert.True(response.CountryId != Guid.Empty);

			Assert.Contains(response, listResponse);
		}

		#endregion

		#region  GetAllCountries

		[Fact]

		public void GetAllCountries_EmptyList()
		{
			//Acts
			List<CountryResponse> actual_country_from_response_list =
				_countriesService.GetAllCountrise();

			//Asserts
			Assert.Empty(actual_country_from_response_list);

		}
		[Fact]
		public void GetAllCountries_AddFewCountries()
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
				response_list.Add(_countriesService.AddCountry(c));
			}

			List<CountryResponse> ListResponse = _countriesService.GetAllCountrise();


			foreach(CountryResponse response in ListResponse)
			{
				Assert.Contains(response, response_list);
			}

		}

		#endregion

		#region GetCountryById

		[Fact]
		public void GetCountryById_NullParam()
		{
			//Arrange
			Guid countryId = Guid.Empty;

			//Act
			CountryResponse? x = _countriesService.GetCountryById(countryId);

			//Assert
			Assert.Null(x);
		}
		[Fact]
		public void GetCountryById_ProperParam()
		{
			//Arrange
			Guid countryId = Guid.NewGuid();

			 CountryAddRequest?  requestCountry = new CountryAddRequest() 
			 {
				 CountryName = "USA"
			 };

			var responseAdd = _countriesService.AddCountry(requestCountry);

			//Act
			CountryResponse? responseGet = _countriesService.GetCountryById(responseAdd.CountryId);

			//Assert
			Assert.Equal(responseAdd, responseGet);
		}

		#endregion


	}
}
