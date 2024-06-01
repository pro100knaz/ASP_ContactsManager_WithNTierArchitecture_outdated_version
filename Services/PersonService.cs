using Enities;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
	public class PersonService : IPersonService
	{
		private readonly PersonsDbContext DbContext;
		private readonly ICountriesService countriesService;

		public PersonService(PersonsDbContext context, ICountriesService countriesService)
		{
			this.DbContext = context;
			this.countriesService = countriesService;
		}

		private PersonResponse ConvertPersonToPersonResponse(Person? person)
		{
			if (person == null)
				return new PersonResponse();

			PersonResponse personResponse = person.ToPersonResponse();
			personResponse.Country = countriesService.GetCountryById(person.CountryId)?.CountryName;
			return personResponse;
		}



		public PersonResponse AddPerson(PersonAddRequest? addPerson)
		{
			if (addPerson is null)
			{
				throw new ArgumentNullException(nameof(addPerson));
			}

			//Model Validation

			//ValidationContext validationContext = new ValidationContext(addPerson);

			//List<ValidationResult> validationResults = new List<ValidationResult>();

			//bool isValid = Validator.TryValidateObject(addPerson, validationContext, validationResults, true);

			//if(!isValid)
			//{
			//	throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
			//}

			//Extension Method(Hand Made)
			ValidationHelper.ModelValidation(addPerson);



			//if (addPerson.PersonName is null)
			//{
			//	throw new ArgumentException(nameof(addPerson.PersonName));
			//}

			var person = addPerson.ToPerson();
			person.Id = Guid.NewGuid();

			var x = DbContext.sp_InsertPerson(person);
			//DbContext.Persons.Add(person);


			DbContext.SaveChanges();

			return ConvertPersonToPersonResponse(person) ?? throw new Exception();

			//PersonResponse personResponse = person.ToPersonResponse();
			//personResponse.Country = countriesService.GetCountryById(person.CountryId)?.CountryName;

			//return personResponse;
		}

		public List<PersonResponse> GetAllPersons()
		{
			//List<PersonResponse> personResponses = new List<PersonResponse>();
			var personResponses = DbContext.Persons.ToList().Select(c => (ConvertPersonToPersonResponse(c))).ToList();
			
			var x = DbContext.sp_GetAllPersons().ToList();
			//{
			//	var res = new PersonResponse();
			//	res = c.ToPersonResponse();
			//	return c.ToPersonResponse();
			//}

			return personResponses ?? new List<PersonResponse>();
		}

		public PersonResponse? GetPerson(Guid? id)
		{

			return (id) == null ? null : ConvertPersonToPersonResponse(DbContext.Persons.FirstOrDefault(c => c.Id == id));
		}

		public List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString)
		{
			var persons = GetAllPersons();

			if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
			{
				return persons;
			}

			switch (searchBy)
			{
				case nameof(PersonResponse.PersonName):
					var x = persons.Where(temp =>
					(!string.IsNullOrEmpty(temp.PersonName)) ?
					temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return x;
				case nameof(PersonResponse.Email):
					persons = persons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Email)) ?
					temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return persons;
				case nameof(PersonResponse.DateOfBirth):
					persons = persons.Where(temp =>
					(temp.DateOfBirth != null) ?
					temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return persons;
				case nameof(PersonResponse.Gender):
					persons = persons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Gender)) ?
					temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return persons;

				case nameof(PersonResponse.Country):
					persons = persons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Country)) ?
					temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return persons;
				case nameof(PersonResponse.Address):
					persons = persons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Address)) ?
					temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) :
					true).ToList();
					return persons;
				default:
					break;


			}
			return persons;


		}

		public List<PersonResponse> GetSortedPersons
			(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrderOptions)
		{
			if (string.IsNullOrEmpty(sortBy))
			{
				return allPersons;
			}
			List<PersonResponse> result = (sortBy, sortOrderOptions) switch
			{
				(nameof(PersonResponse.PersonName), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.PersonName), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),


				(nameof(PersonResponse.Email), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Email), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),



				(nameof(PersonResponse.Age), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Age).ToList(),

				(nameof(PersonResponse.Age), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Age).ToList(),



				(nameof(PersonResponse.Gender), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Gender), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),




				(nameof(PersonResponse.Address), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Address), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),


				(nameof(PersonResponse.Country), SortOrderOptions.Ascending)
			=> allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Country), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),



				(nameof(PersonResponse.ReceiveNewsLatters), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.ReceiveNewsLatters).ToList(),

				(nameof(PersonResponse.ReceiveNewsLatters), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.ReceiveNewsLatters).ToList(),

				_ => allPersons
			};

			return result;

		}

		public PersonResponse UpdatePerson(PersonUpdateRequest? updatePerson)
		{
			if (updatePerson is null)
				throw new ArgumentNullException(nameof(updatePerson));

			ValidationHelper.ModelValidation(updatePerson);

			Person? matchingPerson = DbContext.Persons.FirstOrDefault(temp => temp.Id == updatePerson.PersonId);

			if (matchingPerson is null)
			{
				throw new ArgumentException("given Person Id does not exist");
			}

			//update all deetails

			matchingPerson.PersonName = updatePerson.PersonName;
			matchingPerson.Gender = updatePerson.Gender.ToString();
			matchingPerson.Address = updatePerson.Address;
			matchingPerson.CountryId = updatePerson.CountryId;
			matchingPerson.DateOfBirth = updatePerson.DateOfBirth;
			matchingPerson.Email = updatePerson.Email;
			matchingPerson.ReceiveNewsLatters = updatePerson.ReceiveNewsLatters;

			DbContext.SaveChanges();

			return ConvertPersonToPersonResponse(matchingPerson) ?? throw new Exception();
		}

		public bool DeletePerson(Guid? id)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}

			var x = GetPerson(id);
			if (x is null)
				return false;

			Person? resultPerson = DbContext.Persons.FirstOrDefault(p => p.Id == x.Id);

			if (resultPerson == null)
				return false;

			DbContext.Persons.Remove(
				DbContext.Persons.First(p => p.Id == id));
			DbContext.SaveChanges();

			return true;
		}
	}
}
