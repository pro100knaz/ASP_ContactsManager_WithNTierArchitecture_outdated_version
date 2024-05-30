using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
	public class PersonsController : Controller
	{
        private readonly IPersonService personService;
        private readonly ICountriesService countriesService;
        private readonly ICountriesService countriesServiceTemp;

        public PersonsController(IPersonService personService, ICountriesService countriesService)
        {
            this.personService = personService;
            this.countriesService = countriesService;
		}
        [Route("/persons/index")]
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
	}
}
