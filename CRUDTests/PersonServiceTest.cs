using Enities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		private readonly IPersonService personService;
		private readonly ICountriesService countryService;
		private readonly ITestOutputHelper testOutputHelper;
		private readonly IFixture fixture;

		//constructor
		public PersonServiceTest(ITestOutputHelper testOutputHelper)
		{
			fixture = new Fixture();

			var countriesInitialData = new List<Country>() { };
			var personsInitialData = new List<Person>() { };

			//Craete mock for DbContext
			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
			  new DbContextOptionsBuilder<ApplicationDbContext>().Options
			 );

			//Access Mock DbContext object
			ApplicationDbContext dbContext = dbContextMock.Object;

			//Create mocks for DbSets'
			dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
			dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

			//Create services based on mocked DbContext object
			countryService = new CountryService(dbContext);

			personService = new PersonService(dbContext, countryService);

			this.testOutputHelper = testOutputHelper;
		}



		#region  AddPerson

		[Fact]
		public async Task AddPerson_Null()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;
			//ASSERT AND ACT
			await Assert.ThrowsAsync<ArgumentNullException>( async () => await personService.AddPerson(personAddRequest));
		}
		[Fact]
		public async Task AddPerson_NameIsNull()
		{
			//Arrange
			PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
				.With(temp => temp.PersonName, null as string)
				.Create();
			//ASSERT AND ACT
			 await Assert.ThrowsAsync<ArgumentException>(async () => await personService.AddPerson(personAddRequest));
		}
		[Fact]
		public async Task AddPerson_ProperPersonDetails()
		{
			//Arrange
			PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>().With(a => a.Email, "wadw@mail.ru").Create();

			testOutputHelper.WriteLine(personAddRequest.ToString());

			// ACT
			PersonResponse responseFromAdd = await personService.AddPerson(personAddRequest);

			testOutputHelper.WriteLine(responseFromAdd.ToString());

			PersonResponse? responseFromGet = await personService.GetPerson(responseFromAdd.Id);

			testOutputHelper.WriteLine(responseFromGet?.ToString());
			//ASSERT
			Assert.True(responseFromGet?.Id != Guid.Empty);

			Assert.True(responseFromAdd.Id != Guid.Empty);

			Assert.Equal(responseFromGet, responseFromAdd);

			var x = (await personService.GetAllPersons()).ToList();
			Assert.Contains(responseFromAdd, x);
		}

		#endregion 

		#region GetPersonById
		[Fact]
		public async Task GetPersonById_NullId()
		{
			Guid? id = null;

			PersonResponse? resp = await personService.GetPerson(id);

			Assert.Null(resp);
		}
		[Fact]
		public async Task GetPersonById_ValidId()
		{

			CountryAddRequest countryAddRequest = new CountryAddRequest()
			{
				CountryName = "Canada",
			};
			var CountryResponse = await countryService.AddCountry(countryAddRequest);

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
			PersonResponse personResponseAdd = await personService.AddPerson(personAddRequest);

			PersonResponse? personRequestGet =  await personService.GetPerson(personResponseAdd.Id);

			Assert.Equal(personResponseAdd, personRequestGet);

			Assert.Contains(personRequestGet, await personService.GetAllPersons());
			 
		}
		#endregion

		#region GetAllPersons

		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			var listResp = await personService.GetAllPersons();

			int x = 5;

			Assert.Empty(listResp);
		}

		[Fact]
		public async Task GetAllPersons_GetList()
		{
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


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
				listRespFromAdd.Add( await personService.AddPerson(person));
			}

			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}



			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetAllPersons();

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
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

		#region GetFilteredPersons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchList()
		{
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);

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
				listRespFromAdd.Add(await personService.AddPerson(person));
			}

			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetFilteredPerson(nameof(Person.PersonName), "");

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
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
		[Fact]
		public async Task GetFilteredPersons_ValidSearchList()
		{
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);

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
				listRespFromAdd.Add(await personService.AddPerson(person));
			}

			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}
			const string searchString = "Q";


			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetFilteredPerson(nameof(Person.PersonName), searchString);

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
			foreach (var person in listRespFromGet)
			{
				testOutputHelper.WriteLine(person.ToString());
				if (person.PersonName != null)
				{
					if (person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
					{
						testOutputHelper.WriteLine(person.ToString());
						Assert.Contains(person, listRespFromAdd);
					}
				}

			}
		}

		#endregion 

		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_ValidData()
		{
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);

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
				listRespFromAdd.Add(await personService.AddPerson(person));
			}

			listRespFromAdd = listRespFromAdd.OrderByDescending(temp => temp.PersonName).ToList();

			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}



			//ACT
			List<PersonResponse> listRespFromGet = personService
				.GetSortedPersons(await personService.GetAllPersons(),nameof(Person.PersonName), SortOrderOptions.Descending);

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");

			for (int i = 0; i < listRespFromAdd.Count; i++)
			{
				testOutputHelper.WriteLine(listRespFromAdd[i].ToString());
				Assert.Equal(listRespFromAdd[i], listRespFromGet[i]);
			}
		}
		#endregion

		#region UpdatePerson

		[Fact]
		public async Task UpdatePerson_NullPerson()
		{
			PersonUpdateRequest? personUpdateRequest = null;

			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				await	personService.UpdatePerson(personUpdateRequest);
			});
		}

		[Fact]
		public async Task UpdatePerson_InvalidPersonId()
		{
			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() 
			{
				PersonId = Guid.NewGuid()
			};

			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
			 await	personService.UpdatePerson(personUpdateRequest);
			});
		}

		[Fact]
		public async Task UpdatePerson_PersonNameIsNull()
		{
			#region adding
CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);

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

			List<PersonResponse> personsResponseList = new List<PersonResponse>();
			foreach (var person in personsAddRequests)
			{
				personsResponseList.Add(await personService.AddPerson(person));
			}
			#endregion
			
			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
			{
				PersonId = personsResponseList[0].Id,
				PersonName = null
			};

			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await personService.UpdatePerson(personUpdateRequest);
			});
		}
		[Fact]
		public async Task UpdatePerson_PersonFullDetails()
		{
			#region adding
			CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);

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

			List<PersonResponse> personsResponseList = new List<PersonResponse>();
			foreach (var person in personsAddRequests)
			{
				personsResponseList.Add( await personService.AddPerson(person));
			}
			#endregion

			PersonUpdateRequest? personUpdateRequest = personsResponseList[0].ToPersonUpdateRequest();

			var responseAfterUpdate = await personService.UpdatePerson(personUpdateRequest);

			var responseFromGetAfterUpdate = await personService.GetPerson(responseAfterUpdate.Id);

			Assert.Equal(personsResponseList[0], responseAfterUpdate);
			Assert.Equal(responseFromGetAfterUpdate, responseAfterUpdate);
			Assert.Contains(responseAfterUpdate, await personService.GetAllPersons());
		}

		#endregion

		#region DeletePerson

		[Fact]
		public async Task DeletePerson_ValidPersonId()
		{
			//Arrange 
			CountryAddRequest countryAddRequest = new() { CountryName = "USA" };
			var countryResponseAfterAdd = await countryService.AddCountry(countryAddRequest);

			PersonAddRequest personAddRequest = new PersonAddRequest()
			{
				PersonName = "null",
				Email = "jffgx@mail.com",
				Address = "null",
				DateOfBirth = DateTime.Now,
				CountryId = countryResponseAfterAdd.CountryId,
				Gender = GenderOptions.Male,
				ReceiveNewsLatters = true
			};

			var personResponseFromAdd = await personService.AddPerson(personAddRequest);

			//DELETE PERSON
			//aCT
			bool result = await personService.DeletePerson(personResponseFromAdd.Id);

			//ASSERT
			Assert.True(result);
		}

		[Fact]
		public async Task DeletePerson_InvalidPersonId()
		{
			//Arrange & Act
			bool result = await personService.DeletePerson(Guid.NewGuid());

			//ASSERT
			Assert.False(result);
		}

		#endregion

	}
}
