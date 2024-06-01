using Enities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using System.Text.Json;

namespace CRUDExample.Controllers
{
	[Route("persons")]
	public class PersonsController : Controller
	{
		private readonly IPersonService personService;
		private readonly ICountriesService countriesService;

		public PersonsController(IPersonService personService, ICountriesService countriesService)
		{
			this.personService = personService;
			this.countriesService = countriesService;
		}
		[Route("[action]")]
		[Route("/")]
		public IActionResult Index(string searchBy, string? searchString,
			string sortBy = nameof(PersonResponse.PersonName),
			SortOrderOptions sortOrder = SortOrderOptions.Ascending)
		{
			//Search
			List<PersonResponse> persons = personService.GetFilteredPerson(searchBy, searchString);
			ViewBag.SearchFields = new Dictionary<string, string>()
			{
				{ nameof(PersonResponse.PersonName), "Person Name"},
				{ nameof(PersonResponse.Email), "Email"},
				{ nameof(PersonResponse.DateOfBirth), "Date Of Birth"},
				{ nameof(PersonResponse.Gender), "Gender"},
				{ nameof(PersonResponse.CountryId), "CountryId"},
				{ nameof(PersonResponse.Address), "Address"},
			};


			ViewBag.CurrentSearchBy = searchBy;
			ViewBag.CurrentSearchString = searchString;

			//Sort
			List<PersonResponse> sortedPersons = personService.GetSortedPersons(persons, sortBy, sortOrder);
			ViewBag.CurrentSortBy = sortBy;
			ViewBag.CurrentSortOrder = sortOrder.ToString();

			return View(sortedPersons);
		}
		[Route("[action]")]
		[HttpGet]
		public IActionResult Create()
		{

			var countries = countriesService.GetAllCountrise();

			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryId.ToString()
			});
			//new SelectListItem() {Text = "Rpma",Value = "1"};
			//<option value="1">Rpma</option>


			return View();
		}


		[Route("[action]")]
		[HttpPost]
		public IActionResult Create(PersonAddRequest personAddRequest)
		{
			if (!ModelState.IsValid)
			{
				var countries = countriesService.GetAllCountrise();
				ViewBag.Countries = countries.Select(temp => new SelectListItem()
				{
					Text = temp.CountryName,
					Value = temp.CountryId.ToString()
				});

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

				return View();
			}
			PersonResponse response = personService.AddPerson(personAddRequest);
			return RedirectToAction("Index", "Persons");
		}

		[HttpGet]
		[Route("[action]/{personID}")]
		public IActionResult Edit(Guid personID)
		{
			var country = personService.GetPerson(personID)?.ToPersonUpdateRequest() ?? null;
			if (country == null)
			{
				return RedirectToAction("Index", "Persons");
			}

			var countries = countriesService.GetAllCountrise();
			ViewBag.Countries = countries.Select(temp => new SelectListItem()
			{
				Text = temp.CountryName,
				Value = temp.CountryId.ToString()
			});

			return View(country);
		}

		[HttpPost]
		[Route("[action]/{personID}")]
		public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
		{


			var person = personService.GetPerson(personUpdateRequest.PersonId);
			if (person == null)
			{
				return RedirectToAction("Index", "Persons");
			}

			if (ModelState.IsValid)
			{
				var personResponse = personService.UpdatePerson(personUpdateRequest);
				return RedirectToAction("Index", "Persons");
			}
			else
			{

				var countries = countriesService.GetAllCountrise();
				ViewBag.Countries = countries.Select(temp => new SelectListItem()
				{
					Text = temp.CountryName,
					Value = temp.CountryId.ToString()
				});

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

				return View();
			}

			//var person = personService.GetPerson(personID)?.ToPersonUpdateRequest() ?? null;
			//return RedirectToAction("Index", "Persons");
		}



		[HttpGet]
		[Route("[action]/{personID}")]
		public IActionResult Delete(Guid personID)
		{
			var person = personService.GetPerson(personID);
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
		public IActionResult Delete(PersonResponse personResponse)
		{

			var person1 = personService.GetPerson(personResponse.Id);
			if (person1 == null)
			{
				return RedirectToAction("Index", "Persons");
			}
			else
			{

				var person = personService.DeletePerson(personResponse.Id);



				return RedirectToAction("Index", "Persons");
			}

			//var person1 = personService.GetPerson(personID)?.ToPersonUpdateRequest() ?? null;
			//return RedirectToAction("Index", "Persons");
		}


	}
}
