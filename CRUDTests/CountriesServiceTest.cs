
using AutoFixture;
using Enities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts.interfaces;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{


	public class CountriesServiceTest
	{
		private readonly ICountriesService _countriesService;
		private readonly Mock<ICountriesRepository> mockCountryRepository;
		private readonly ICountriesRepository countryRepository;

		private readonly IFixture fixture;

		public CountriesServiceTest()
		{
			fixture = new Fixture();

			mockCountryRepository = new Mock<ICountriesRepository>();
			countryRepository = mockCountryRepository.Object;

			_countriesService = new CountryService(countryRepository);
		}
		// null - throw ARGUM

		#region  AddCountry


		[Fact]
		public async Task AddCountry_NullCountry_ToBeArgumentNullException()
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
		public async Task AddCountry_CountryNameIsNull_ToBeArgumentNullException()
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
		public async Task AddCountry_CountryNameIsDuplicate_ToBeArgumentException()
		{
			//Arrange
			Country country = GetCountry();

			CountryAddRequest? request2 = new CountryAddRequest() { CountryName = country.Name };

			CountryAddRequest? request1 = new CountryAddRequest() { CountryName = country.Name };

			mockCountryRepository.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
			mockCountryRepository.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country);
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
			Country country = GetCountry();

			CountryAddRequest? request = new CountryAddRequest() { CountryName = country.Name };

			mockCountryRepository.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);

			mockCountryRepository.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
			//Act
			CountryResponse response_expected = country.ToCountryResponse();

			CountryResponse response_from_add = await _countriesService.AddCountry(request);
			response_expected.CountryId = response_from_add.CountryId;

			//var x = 6;
			//Assert
			Assert.True(response_from_add.CountryId != Guid.Empty);

			response_expected.Should().Be(response_from_add);

		}

		#endregion

		#region  GetAllCountries

		[Fact]

		public async Task GetAllCountries_EmptyList()
		{
			List<Country> countries = new();

			List<CountryResponse> response_list_expected = countries.Select(country => country.ToCountryResponse()).ToList();

			mockCountryRepository.Setup(temp => temp.GetCountries()).ReturnsAsync(countries);

			List<CountryResponse> ListResponse = await _countriesService.GetAllCountrise();
			//Acts
			List<CountryResponse> actual_country_from_response_list =
				await _countriesService.GetAllCountrise();

			//Asserts
			Assert.Empty(actual_country_from_response_list);

		}
		[Fact]
		public async Task GetAllCountries_AddFewCountries()
		{
			//Arrange
			List<Country> countries = new()
			{
				GetCountry(),
				GetCountry(),
				GetCountry(),
				GetCountry(),
				GetCountry(),
			};


			List<CountryResponse> response_list_expected = countries.Select(country => country.ToCountryResponse()).ToList();

			mockCountryRepository.Setup(temp => temp.GetCountries()).ReturnsAsync(countries);

			List<CountryResponse> ListResponse = await _countriesService.GetAllCountrise();


			ListResponse.Should().BeEquivalentTo(response_list_expected);
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
			Country country = GetCountry();

			CountryAddRequest? requestCountry = new CountryAddRequest()
			{
				CountryName = country.Name
			};

			CountryResponse countryResponse_expected = country.ToCountryResponse();

			mockCountryRepository.Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>())).ReturnsAsync(country);
			//Act
			CountryResponse? responseGet = await _countriesService.GetCountryById(country.CountryId);
			countryResponse_expected.CountryId = responseGet.CountryId;
			//Assert
			Assert.Equal(countryResponse_expected, responseGet);
		}

		#endregion

		#region Tools Methods

		Country GetCountry()
		{
			return fixture.Build<Country>().With(temp => temp.Persons, null as ICollection<Person>).Create();
		}

		#endregion

	}
}
