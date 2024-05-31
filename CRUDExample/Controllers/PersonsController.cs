using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;

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

			ViewBag.Countries = countries;

			return View();
		}


		[Route("[action]")]
		[HttpPost]
		public IActionResult Create(PersonAddRequest personAddRequest)
		{
			if(!ModelState.IsValid)
			{
				var countries = countriesService.GetAllCountrise();
				ViewBag.Countries = countries;

				ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

				return View();
			}
			PersonResponse response = personService.AddPerson(personAddRequest);
			return RedirectToAction("Index", "Persons");
		}
	}
}
