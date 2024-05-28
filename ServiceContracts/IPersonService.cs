using ServiceContracts.DTO.PersonDto;
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

	}
}
