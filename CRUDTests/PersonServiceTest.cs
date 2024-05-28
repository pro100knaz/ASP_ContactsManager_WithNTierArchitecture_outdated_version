using Enities;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		private readonly IPersonService personService;

		public PersonServiceTest()
        {
			personService = new PersonService();
		}


		[Fact]
		public void AddPerson_Null()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;
			//ASSERT AND ACT
			 Assert.Throws<ArgumentNullException>(()=> personService.AddPerson(personAddRequest));
		}
		[Fact]
		public void AddPerson_NameIsNull()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new ()
			{ 
				PersonName = null,

			};
			//ASSERT AND ACT
			Assert.Throws<ArgumentException>(() => personService.AddPerson(personAddRequest));
		}
		[Fact]
		public void AddPerson_ProperPersonDetails()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new()
			{
				PersonName = "null",
				Email= "@null",
				Address= "null",
				DateOfBirth = DateTime.Now,
				CountryId = Guid.NewGuid(),
				Gender = GenderOptions.Male,
				ReceiveNewsLatters = true
			};
			// ACT
			PersonResponse responseFromAdd = personService.AddPerson(personAddRequest);

			PersonResponse responseFromGet = personService.GetPerson(responseFromAdd.Id);
			//ASSERT
			Assert.True(responseFromGet.Id != Guid.Empty);

			Assert.True(responseFromAdd.Id != Guid.Empty);

			Assert.Equal(responseFromGet, responseFromAdd);

			Assert.Contains(responseFromAdd, personService.GetAllPersons());
		}
	}
}
