using CRUDExample.Filters.ActionFilters;
using Enities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using System.Text.Json;

namespace CRUDExample.Controllers
{
	[Route("persons")]
	[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[]
		{
			"X-Custom-Key-From-Class","Custom-Value-From-Class", 8   //first value will be supplied autu by ioc but another we can supply personaly
		})] //Just Can And Do and add Filter FOR EVERY METHOD inside that controller
	public class PersonsController : Controller
	{
		private readonly ILogger<PersonsController> logger;
		private readonly IPersonService personService;
		private readonly ICountriesService countriesService;

		public PersonsController(ILogger<PersonsController> logger,
			IPersonService personService, ICountriesService countriesService)
		{
			this.logger = logger;
			this.personService = personService;
			this.countriesService = countriesService;
		}


		[Route("[action]")]
		[Route("/")]
		[TypeFilter(typeof(PersonsListActionFilter))] // personal filter
		[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[]
		{
			"X-Custom-Key-From-Action","Custom-Value-From-Action", 3   //first value will be supplied autu by ioc but another we can supply personaly
		} //, Order = 3  //Oorder like 0 , 1, 2 , 3 action 3, 2, 1, 0 )) easy and clear
			)] 
		public async Task<IActionResult> Index(string searchBy, string? searchString,
			string sortBy = nameof(PersonResponse.PersonName),
			SortOrderOptions sortOrder = SortOrderOptions.Ascending)
		{
			//Search
			List<PersonResponse> persons = (!string.IsNullOrEmpty(searchString)) 
				? await personService.GetFilteredPerson(searchBy, searchString) : await personService.GetAllPersons();

			//Sort
			List<PersonResponse> sortedPersons = await personService.GetSortedPersons(persons, sortBy, sortOrder);

			return View(sortedPersons);
		}


		[Route("[action]")]
		[HttpGet]
		public async Task<IActionResult> Create()
		{

			var countries = await countriesService.GetAllCountrise();

			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryId.ToString()
			});


			return View();
		}


		[Route("[action]")]
		[HttpPost]
		[TypeFilter(typeof(PersonCreateAndEditPostCreateActionFilter))]
		public async Task<IActionResult> Create(PersonAddRequest personRequest)
		{
			#region Хлам
//if (!ModelState.IsValid)
			//{
			//	var countries = await countriesService.GetAllCountrise();
			//	ViewBag.Countries = countries.Select(temp => new SelectListItem()
			//	{
			//		Text = temp.CountryName,
			//		Value = temp.CountryId.ToString()
			//	});

			//	ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

			//	return View(personRequest);
			//}

			//Now it is inside filters
			#endregion
			PersonResponse response = await personService.AddPerson(personRequest);
			return RedirectToAction("Index", "Persons");
		}


		[HttpGet]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Edit(Guid personID)
		{
			var country = (await personService.GetPerson(personID))?.ToPersonUpdateRequest() ?? null;
			if (country == null)
			{
				return RedirectToAction("Index", "Persons");
			}

			var countries = await countriesService.GetAllCountrise();
			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryId.ToString()
			});

			return View(country);
		}


		[HttpPost]
		[Route("[action]/{personID}")]
		[TypeFilter(typeof(PersonCreateAndEditPostCreateActionFilter))]
		public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
		{


			var person = await personService.GetPerson(personRequest.PersonId);
			if (person == null)
			{
				return RedirectToAction("Index", "Persons");
			}
			//validation will be inside filters

			var personResponse = await personService.UpdatePerson(personRequest);
			return RedirectToAction("Index", "Persons");

			#region Хлам
//else
			//{

			//	var countries = await countriesService.GetAllCountrise();
			//	ViewBag.Countries = countries.Select(temp => new SelectListItem()
			//	{
			//		Text = temp.CountryName,
			//		Value = temp.CountryId.ToString()
			//	});

			//	ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

			//	return View(personRequest);
			//}

			//var person = personService.GetPerson(personID)?.ToPersonUpdateRequest() ?? null;
			//return RedirectToAction("Index", "Persons");
			//return View(personRequest);
			#endregion
		}


		[HttpGet]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Delete(Guid personID)
		{
			var person = await personService.GetPerson(personID);
			if (person == null)
			{
				return RedirectToAction("Index", "Persons");
			}

			//var countries = countriesService.GetAllCountrise();
			//ViewBag.Countries = countries.Select(temp => new SelectListItem()
			//{
			//	Text = temp.CountryName,
			//	Value = temp.CountryId.ToString()
			//});

			return View(person);
		}


		[HttpPost]
		[Route("[action]/{personID}")]
		public async Task<IActionResult> Delete(PersonResponse personResponse)
		{

			var person1 = await personService.GetPerson(personResponse.Id);
			if (person1 == null)
			{
				return RedirectToAction("Index", "Persons");
			}
			else
			{

				var person = await personService.DeletePerson(personResponse.Id);



				return RedirectToAction("Index", "Persons");
			}

			//var person1 = personService.GetPerson(personID)?.ToPersonUpdateRequest() ?? null;
			//return RedirectToAction("Index", "Persons");
		}


		[Route("PersonsPdf")]
		public async Task<IActionResult> PersonsPdf()
		{
			var persons = await personService.GetAllPersons();

			return new ViewAsPdf("PersonsPdf", persons, ViewData)
			{
				PageMargins = new Rotativa.AspNetCore.Options.Margins()
				{
					Top = 20,
					Right = 20,
					Bottom = 20,
					Left = 20
				},
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,

			};

		}


		[Route("PersonsCSV")]
		public async Task<IActionResult> PersonsCSV()
		{
			var memoryStream = await personService.GetPersonCSV();
			return File(memoryStream, "application/octet-stream", "persons.csv");
		}


		[Route("[action]")]
		public async Task<IActionResult> PersonsExcel()
		{
			var memoryStream = await personService.GetPersonExcel();
			return File(memoryStream, "\tapplication/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
		}

	}
}
