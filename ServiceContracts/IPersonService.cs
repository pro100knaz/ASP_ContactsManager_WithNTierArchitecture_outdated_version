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
		Task <PersonResponse> AddPerson(PersonAddRequest? addPerson);
		Task<PersonResponse?> GetPerson(Guid? id);
		Task<List<PersonResponse>> GetAllPersons();
		Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString);

		public  Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy,
			SortOrderOptions sortOrderOptions);
		/// <summary>
		/// Updates the person data
		/// </summary>
		/// <param name="updatePerson"></param>
		/// <returns></returns>
		Task<PersonResponse> UpdatePerson(PersonUpdateRequest? updatePerson);

		Task<bool> DeletePerson(Guid? id);
		/// <summary>
		/// Returns Persons as CSV
		/// </summary>
		/// <returns>Retuen the stream with CSV</returns>
		Task<MemoryStream> GetPersonCSV();
		Task<MemoryStream> GetPersonExcel();
	}
}
