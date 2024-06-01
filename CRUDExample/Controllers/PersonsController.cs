using Enities;
using Microsoft.AspNetCore.Mvc;
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
		public async Task<IActionResult> Index(string searchBy, string? searchString,
			string sortBy = nameof(PersonResponse.PersonName),
			SortOrderOptions sortOrder = SortOrderOptions.Ascending)
		{
			//Search
			List<PersonResponse> persons = await personService.GetFilteredPerson(searchBy, searchString);
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
		public async Task<IActionResult> Create()
		{

			var countries = await countriesService.GetAllCountrise();

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
		public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
		{
			if (!ModelState.IsValid)
			{
				var countries = await countriesService.GetAllCountrise();
				ViewBag.Countries = countries.Select(temp => new SelectListItem()
				{
					Text = temp.CountryName,
					Value = temp.CountryId.ToString()
				});

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

				return View();
			}
			PersonResponse response = await personService.AddPerson(personAddRequest);
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
		public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
		{


			var person =  await personService.GetPerson(personUpdateRequest.PersonId);
			if (person == null)
			{
				return RedirectToAction("Index", "Persons");
			}

			if (ModelState.IsValid)
			{
				var personResponse = await personService.UpdatePerson(personUpdateRequest);
				return RedirectToAction("Index", "Persons");
			}
			else
			{

				var countries = await countriesService.GetAllCountrise();
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
					Top = 20, Right=20, Bottom = 20,
					Left = 20
				},
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,

			};

		}

	}
}
