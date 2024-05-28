using Enities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		private readonly IPersonService personService;
		private readonly ICountriesService countryService;
		private readonly ITestOutputHelper testOutputHelper;

		public PersonServiceTest( ITestOutputHelper testOutputHelper)
		{
			personService = new PersonService();
			countryService = new CountryService();
			this.testOutputHelper = testOutputHelper;
		}
		#region  AddPerson

			[Fact]
			public void AddPerson_Null()
			{
				//Arrange
				PersonAddRequest? personAddRequest = null;
				//ASSERT AND ACT
				Assert.Throws<ArgumentNullException>(() => personService.AddPerson(personAddRequest));
			}
			[Fact]
			public void AddPerson_NameIsNull()
			{
				//Arrange
				PersonAddRequest? personAddRequest = new()
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
					Email = "jffgx@mail.com",
					Address = "null",
					DateOfBirth = DateTime.Now,
					CountryId = Guid.NewGuid(),
					Gender = GenderOptions.Male,
					ReceiveNewsLatters = true
				};
				// ACT
				PersonResponse responseFromAdd = personService.AddPerson(personAddRequest);

				PersonResponse? responseFromGet = personService.GetPerson(responseFromAdd.Id);
				//ASSERT
				Assert.True(responseFromGet?.Id != Guid.Empty);

				Assert.True(responseFromAdd.Id != Guid.Empty);

				Assert.Equal(responseFromGet, responseFromAdd);

				Assert.Contains(responseFromAdd, personService.GetAllPersons());
			}

		#endregion

		#region GetPersonById
		[Fact]
		public void GetPersonById_NullId()
		{
			Guid? id = null;

			PersonResponse? resp = personService.GetPerson(id);

			Assert.Null(resp);
		}
		[Fact]
		public void GetPersonById_ValidId()
		{

			CountryAddRequest countryAddRequest = new CountryAddRequest()
			{
				CountryName = "Canada",
			};
			var CountryResponse = countryService.AddCountry(countryAddRequest);

			PersonAddRequest personAddRequest = new()
			{
				PersonName = "null",
				Email = "jffgx@sample.com",
				CountryId = CountryResponse.CountryId,
				Address = "address",
				ReceiveNewsLatters = true,
				DateOfBirth = DateTime.Now,
				Gender = GenderOptions.Male
			};
			PersonResponse personResponseAdd = personService.AddPerson(personAddRequest);

			PersonResponse? personRequestGet = personService.GetPerson(personResponseAdd.Id);

			Assert.Equal(personResponseAdd, personRequestGet);

			Assert.Contains(personRequestGet, personService.GetAllPersons());

		}
		#endregion

		#region GetAllPersons

		[Fact]
		public void GetAllPersons_EmptyList()
		{
			List<PersonResponse> listResp = personService.GetAllPersons();
			Assert.Empty(listResp);
		}

		[Fact]
		public void GetAllPersons_GetList()
		{


			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA"};
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = countryService.AddCountry(countryAddRequest1);
			var c2 = countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>() 
			{
				 new PersonAddRequest()
				{
					PersonName = "null",
					Email = "jffgx@mail.com",
					Address = "null",
					DateOfBirth = DateTime.Now,
					CountryId = c1.CountryId,
					Gender = GenderOptions.Male,
					ReceiveNewsLatters = true
				},
				 new PersonAddRequest()
				{
					PersonName = "QWE",
					Email = "jfQWEfgx@mail.com",
					Address = "QWEQ",
					DateOfBirth = DateTime.Now,
					CountryId = c2.CountryId,
					Gender = GenderOptions.Male,
					ReceiveNewsLatters = true
				},

				  new PersonAddRequest()
				{
					PersonName = "QQWEWE",
					Email = "jfQWEfgQEQx@mail.com",
					Address = "QWQWEEQ",
					DateOfBirth = DateTime.Now,
					CountryId = c1.CountryId,
					Gender = GenderOptions.Male,
					ReceiveNewsLatters = true
				},
			};



			List<PersonResponse> listRespFromAdd = new List<PersonResponse>();
			foreach (var person in personsAddRequests)
			{
				listRespFromAdd.Add(personService.AddPerson(person));
			}



			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}


			//ACT
			List<PersonResponse> listRespFromGet = personService.GetAllPersons();
			
			testOutputHelper.WriteLine("\n" +"	Received List(from get all) : \n");
			foreach (var person in listRespFromGet)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}


			//Assert
			foreach (var person in listRespFromGet)
			{
				Assert.Contains(person, listRespFromAdd);
			}
		}



		#endregion

	}
}
