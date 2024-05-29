using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface IPersonService
	{
		PersonResponse AddPerson(PersonAddRequest? addPerson);
		PersonResponse? GetPerson(Guid? id);
		List<PersonResponse> GetAllPersons();
		List<PersonResponse> GetFilteredPerson(string searchBy, string? searchString);

		public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy,
			SortOrderOptions sortOrderOptions);
		/// <summary>
		/// Updates the person data
		/// </summary>
		/// <param name="updatePerson"></param>
		/// <returns></returns>
		PersonResponse UpdatePerson(PersonUpdateRequest? updatePerson);

		bool DeletePerson(Guid? id);
	}
}
