using Enities;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
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
		private readonly List<Person> persons;
		private readonly ICountriesService countriesService;
		public PersonService()
        {
			persons = new List<Person>();
			countriesService = new CountryService();
		}

		private PersonResponse ConvertPersonToPersonResponse(Person person)
		{
			PersonResponse personResponse = person.ToPersonResponse();
			personResponse.Country = countriesService.GetCountryById(person.CountryId)?.CountryName;
			return personResponse;
		}

        public PersonResponse AddPerson(PersonAddRequest? addPerson)
		{
			if(addPerson is null)
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

			persons.Add(person);

			return ConvertPersonToPersonResponse(person);

			//PersonResponse personResponse = person.ToPersonResponse();
			//personResponse.Country = countriesService.GetCountryById(person.CountryId)?.CountryName;

			//return personResponse;
		}

		public List<PersonResponse> GetAllPersons()
		{
			List<PersonResponse> personResponses = new List<PersonResponse>();
			personResponses = persons.Select(c => c.ToPersonResponse()).ToList();
			//{
			//	var res = new PersonResponse();
			//	res = c.ToPersonResponse();
			//	return c.ToPersonResponse();
			//}

			return personResponses;
		}

		public PersonResponse? GetPerson(Guid? id)
		{
			
			return persons?.FirstOrDefault(c => c.Id == id)?.ToPersonResponse() ?? null;
		}
	}
}
