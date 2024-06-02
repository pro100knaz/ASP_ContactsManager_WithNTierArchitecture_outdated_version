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
using FluentAssertions;

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

			Func<Task> action = async () =>
			{
				await personService.AddPerson(personAddRequest);
			};
			
			await action.Should().ThrowAsync<ArgumentNullException>();

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
			Func<Task> action = async () =>
			{
				await personService.AddPerson(personAddRequest);
			};

			await action.Should().ThrowAsync<ArgumentException>();


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

			responseFromGet?.Id.Should().NotBe(Guid.Empty);


			Assert.True(responseFromAdd.Id != Guid.Empty);

			responseFromAdd.Id.Should().NotBe(Guid.Empty);

			Assert.Equal(responseFromGet, responseFromAdd);

			responseFromAdd.Should().Be(responseFromGet);

			var x = await personService.GetAllPersons();




			Assert.Contains(responseFromAdd, x);
		}

		#endregion 

		#region GetPersonById
		[Fact]
		public async Task GetPersonById_NullId()
		{
			Guid? id = null;

			PersonResponse? resp = await personService.GetPerson(id);


			resp.Should().BeNull();

			Assert.Null(resp);
		}
		[Fact]
		public async Task GetPersonById_ValidId()
		{

			CountryAddRequest countryAddRequest = GetAddCountryRequest();

			var CountryResponse = await countryService.AddCountry(countryAddRequest);

			PersonAddRequest personAddRequest = GetPersonAddRequest(CountryResponse); 

			PersonResponse personResponseAdd = await personService.AddPerson(personAddRequest);

			PersonResponse? personRequestGet =  await personService.GetPerson(personResponseAdd.Id);

			Assert.Equal(personResponseAdd, personRequestGet);
			personResponseAdd.Should().Be(personRequestGet);


			Assert.Contains(personRequestGet, await personService.GetAllPersons());

			 (await personService.GetAllPersons()).Should().Contain(personRequestGet!);
			 (await personService.GetAllPersons()).Should().Contain(personResponseAdd);
		}
		#endregion

		#region GetAllPersons

		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			var listResp = await personService.GetAllPersons();

			listResp.Should().BeEmpty();

			Assert.Empty(listResp);
		}

		[Fact]
		public async Task GetAllPersons_GetList()
		{
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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
			//foreach (var person in listRespFromGet)
			//{
			//	Assert.Contains(person, listRespFromAdd);

			//	listRespFromAdd.Should().Contain(person);
			//}

			listRespFromAdd.Should().BeEquivalentTo(listRespFromGet);

		}

		#endregion

		#region GetFilteredPersons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchList()
		{
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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

				listRespFromAdd.Should().Contain(person);
			}


		//	listRespFromGet.Should().BeEquivalentTo(personsAddRequests);

		}
		[Fact]
		public async Task GetFilteredPersons_ValidSearchList()
		{
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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
			const string searchString = "32";

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
						listRespFromAdd.Should().Contain(person);
					}
				}
			}

			//listRespFromGet.Should().BeEquivalentTo(listRespFromAdd);

			listRespFromGet.Should().OnlyContain(temp => temp.PersonName!.Contains(searchString, StringComparison.OrdinalIgnoreCase));


		}

		#endregion 

		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_ValidData()
		{
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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
				listRespFromAdd[i].Should().Be(listRespFromGet[i]);
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
				await personService.UpdatePerson(personUpdateRequest);
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
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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
			CountryAddRequest countryAddRequest1 = GetAddCountryRequest();
			CountryAddRequest countryAddRequest2 = GetAddCountryRequest();

			var c1 = await countryService.AddCountry(countryAddRequest1);
			var c2 = await countryService.AddCountry(countryAddRequest2);


			List<PersonAddRequest> personsAddRequests = new List<PersonAddRequest>()
			{
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c2),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
				 GetPersonAddRequest(c1),
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
			CountryAddRequest countryAddRequest = GetAddCountryRequest();
			var countryResponseAfterAdd = await countryService.AddCountry(countryAddRequest);

			PersonAddRequest personAddRequest = GetPersonAddRequest(countryResponseAfterAdd);

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

		#region Tool Methods

		private CountryAddRequest GetAddCountryRequest()
		{
			return fixture.Build<CountryAddRequest>().Create();
		}
		
		private PersonAddRequest GetPersonAddRequest(CountryResponse countryResponse)
		{
			return fixture.Build<PersonAddRequest>().With(temp => temp.Email, "qqweewq@mail.ru").With(temp => temp.CountryId, countryResponse.CountryId).Create();
		}

		#endregion
	}
}
