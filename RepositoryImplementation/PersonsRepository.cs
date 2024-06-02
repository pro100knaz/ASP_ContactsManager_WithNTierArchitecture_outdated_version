using Enities;
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
		public Task<Person> AddPerson(Person person)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeletePersonByPersonId(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Person>> GetAllPersons()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Person>> GetFilteredPerson(Expression<Func<Person, bool>> predicate)
		{
			throw new NotImplementedException();
		}

		public Task<Person?> GetPersonById(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<Person> GetPersonByName(string name)
		{
			throw new NotImplementedException();
		}

		public Task<Person> UpdatePersonByPersonId(Person person)
		{
			throw new NotImplementedException();
		}
	}
}
