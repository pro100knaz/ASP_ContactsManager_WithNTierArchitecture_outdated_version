using Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesImplementation
{
	public class PersonsRepository : IPersonsRepository
	{
		private readonly ILogger<PersonsRepository> logger;
		private readonly ApplicationDbContext applicationDbContext;

		public PersonsRepository(ILogger<PersonsRepository> logger, ApplicationDbContext applicationDbContext)
        {
			this.logger = logger;
			this.applicationDbContext = applicationDbContext;
		}
        public async Task<Person> AddPerson(Person person)
		{
			await applicationDbContext.Persons.AddAsync(person);
			await applicationDbContext.SaveChangesAsync();
			return person;
		}

		public async Task<bool> DeletePersonByPersonId(Guid id)
		{
			applicationDbContext.Persons.RemoveRange(applicationDbContext.Persons.Where(temp => temp.Id == id));

			int rowsDeleted = await applicationDbContext.SaveChangesAsync();

			return rowsDeleted > 0;
		}

		public async Task<List<Person>> GetAllPersons()
		{
			logger.LogInformation("Inside \"Get All Persons \" of Person Repository");
			return await applicationDbContext.Persons.AsNoTracking().Include("Country").ToListAsync();

		}

		public async Task<List<Person>> GetFilteredPerson(Expression<Func<Person, bool>> predicate)
		{
			logger.LogInformation("Inside \"Get Filtered Person \" of Person Repository");
			return await applicationDbContext.Persons.AsNoTracking().Include("Country").Where(predicate).ToListAsync();
		}

		public async Task<Person?> GetPersonById(Guid id)
		{
			return await applicationDbContext.Persons.AsNoTracking().FirstOrDefaultAsync(person => person.Id == id);
		}

		public Task<Person> GetPersonByName(string name)
		{
			throw new NotImplementedException();
		}

		public async Task<Person> UpdatePersonByPerson(Person person)
		{
			Person? matchingPerson  = await applicationDbContext.Persons.FirstOrDefaultAsync(temp => temp.Id == person.Id);

			if (matchingPerson == null)
				return person;

			 
			matchingPerson.Id = person.Id;
			matchingPerson.Address = person.Address;
			matchingPerson.Gender = person.Gender;
			matchingPerson.DateOfBirth = person.DateOfBirth;
			matchingPerson.CountryId = person.CountryId;
			matchingPerson.Email = person.Email;
			matchingPerson.ReceiveNewsLatters = person.ReceiveNewsLatters;


			int count = await applicationDbContext.SaveChangesAsync();

			return matchingPerson;
		}
	}
}
