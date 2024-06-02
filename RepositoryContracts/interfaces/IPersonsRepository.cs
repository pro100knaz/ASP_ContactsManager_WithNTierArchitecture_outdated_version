using Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.interfaces
{
	public interface IPersonsRepository
	{
		Task<Person> AddPerson(Person person);

		Task<IEnumerable<Person>> GetAllPersons();

		Task<IEnumerable<Person>> GetFilteredPerson(Expression<Func<Person, bool>> predicate);

		Task<Person?> GetPersonById(Guid id);

		Task<Person> GetPersonByName(string name);

		Task<bool> DeletePersonByPersonId(Guid id);

		Task<Person> UpdatePersonByPersonId(Person person);

	}
}
