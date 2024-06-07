using AutoFixture;
using Castle.Core.Logging;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
	public class PersonsControllerTest
	{
		private readonly IPersonService personService;
		private readonly ICountriesService countryService;

		private readonly ILogger<PersonsController> logger;

		private readonly Mock<IPersonService> mockPersonService;
		private readonly Mock<ICountriesService> mockCountryService;

		private readonly Mock<ILogger<PersonsController>> mockLogger;

		private readonly IFixture fixture;

        public PersonsControllerTest()
        {
            fixture = new Fixture();

			mockPersonService = new Mock<IPersonService>();
			mockCountryService = new Mock<ICountriesService>();

			mockLogger = new Mock<ILogger<PersonsController>>();

			personService = mockPersonService.Object;
			countryService = mockCountryService.Object;

			logger = mockLogger.Object;
        }

		#region Index
		[Fact]
		public async Task Index_ShouldReturnIndexViewWithPersonsList()
		{
			//Arrange 
			List<PersonResponse> personResponses = fixture.Create<List<PersonResponse>>();

			PersonsController personsController = new PersonsController(logger, personService, countryService);

			mockPersonService
				.Setup(temp => temp.GetFilteredPerson
				(It.IsAny<string>(),
				It.IsAny<string>()))
				.ReturnsAsync(personResponses);

			mockPersonService
				.Setup(temp => temp.GetSortedPersons
				(It.IsAny<List<PersonResponse>>(),
				It.IsAny<string>(),
				It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponses);

			//Act
			IActionResult result = await personsController.Index(fixture.Create<string>(),
				fixture.Create<string>(), fixture.Create<string>(),
				fixture.Create<SortOrderOptions>());

			//Assert
			ViewResult viewResult = Assert.IsType<ViewResult>(result);

			viewResult.ViewData.Model.Should().NotBeNull();
			viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
			viewResult.ViewData.Model.Should().Be(personResponses);
		}

		#endregion


		#region Create

		//[Fact]
		//public async Task Create_IfModelErrors_ToReturnCreateView()
		//{
		//	//Arrange 
		//	List<PersonResponse> personResponses = fixture.Create<List<PersonResponse>>();

		//	PersonResponse personResponse = fixture.Create<PersonResponse>();


		//	PersonAddRequest person_Add_Requests = fixture.Create<PersonAddRequest>();

		//	List<CountryResponse> countryResponses = fixture.Create<List<CountryResponse>>();

		//	PersonsController personsController = new PersonsController(logger,personService, countryService);


		//	mockCountryService.Setup(
		//		temp => temp.GetAllCountrise())
		//		.ReturnsAsync(countryResponses);


		//	mockPersonService.Setup(
		//		temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
		//		.ReturnsAsync(personResponse);

		//	//mockPersonService
		//	//	.Setup(temp => temp.GetFilteredPerson
		//	//	(It.IsAny<string>(),
		//	//	It.IsAny<string>()))
		//	//	.ReturnsAsync(personResponses);

		//	//mockPersonService
		//	//	.Setup(temp => temp.GetSortedPersons
		//	//	(It.IsAny<List<PersonResponse>>(),
		//	//	It.IsAny<string>(),
		//	//	It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponses);

		//	//Act
		//	personsController.ModelState.AddModelError("PersonName", "Person Name can not be blank");

		//	IActionResult result = await personsController.Create(person_Add_Requests);

		//	//Assert
		//	ViewResult viewResult = Assert.IsType<ViewResult>(result);

		//	viewResult.ViewData.Model.Should().NotBeNull();

		//	viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();


		//	viewResult.ViewData.Model.Should().Be(person_Add_Requests);
		//}

		//It is Invalid anymore becouse responsability to check validat Model state is a filter duty

		[Fact]
		public async Task Create_IfNoModelErrors_ToReturnRedirectIndex()
		{
			//Arrange 
			List<PersonResponse> personResponses = fixture.Create<List<PersonResponse>>();

			PersonResponse personResponse = fixture.Create<PersonResponse>();


			PersonAddRequest person_Add_Requests = fixture.Create<PersonAddRequest>();

			List<CountryResponse> countryResponses = fixture.Create<List<CountryResponse>>();

			PersonsController personsController = new PersonsController(logger, personService, countryService);


			mockCountryService.Setup(
				temp => temp.GetAllCountrise())
				.ReturnsAsync(countryResponses);


			mockPersonService.Setup(
				temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
				.ReturnsAsync(personResponse);

			//mockPersonService
			//	.Setup(temp => temp.GetFilteredPerson
			//	(It.IsAny<string>(),
			//	It.IsAny<string>()))
			//	.ReturnsAsync(personResponses);

			//mockPersonService
			//	.Setup(temp => temp.GetSortedPersons
			//	(It.IsAny<List<PersonResponse>>(),
			//	It.IsAny<string>(),
			//	It.IsAny<SortOrderOptions>())).ReturnsAsync(personResponses);

			//Act
			//personsController.ModelState.AddModelError("PersonName", "Person Name can not be blank");

			IActionResult result = await personsController.Create(person_Add_Requests);

			//Assert
			RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

			redirectResult.ActionName.Should().Be("Index");


		}
		#endregion

	}
}
