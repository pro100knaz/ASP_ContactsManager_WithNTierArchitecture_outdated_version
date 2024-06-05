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
using RepositoryContracts.interfaces;
using Moq;
using RepositoriesImplementation;
using FluentAssertions.Execution;
using System.Linq.Expressions;
using Serilog.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
	public class PersonServiceTest
	{
		private readonly IPersonService personService;
		private readonly ITestOutputHelper testOutputHelper;
		private readonly IFixture fixture;
		//repositories
		private readonly IPersonsRepository personsRepository;
		private readonly Mock<IPersonsRepository> mockPersonsRepository;

		//constructor
		public PersonServiceTest(ITestOutputHelper testOutputHelper)
		{
			fixture = new Fixture();
			mockPersonsRepository = new Mock<IPersonsRepository>();
			personsRepository = mockPersonsRepository.Object;
			var diagnosticContextMock = new Mock<IDiagnosticContext>();
			var loggerMock = new Mock<ILogger<PersonService>>();
			personService = new PersonService(diagnosticContextMock.Object, loggerMock.Object, personsRepository);

			this.testOutputHelper = testOutputHelper;
		}

		#region  AddPerson

		[Fact]
		public async Task AddPerson_Null()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;
			//ASSERT AND ACT

			//no need to mock 
			//mockPersonsRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(null);

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


			Person person = personAddRequest.ToPerson();
			mockPersonsRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

			//ASSERT AND ACT
			Func<Task> action = async () =>
			{
				await personService.AddPerson(personAddRequest);
			};

			await action.Should().ThrowAsync<ArgumentException>();


			await Assert.ThrowsAsync<ArgumentException>(async () => await personService.AddPerson(personAddRequest));
		}
		[Fact]
		public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
		{
			//Arrange
			PersonAddRequest? personAddRequest = fixture.Build<PersonAddRequest>().With(a => a.Email, "wadw@mail.ru").Create();

			testOutputHelper.WriteLine(personAddRequest.ToString());

			Person person = personAddRequest.ToPerson();
			PersonResponse personResponse_expected = person.ToPersonResponse();

			//iF WE SUPPLY ANY ARGUMENT VALUE TO THE ADDpERSON METHOD , IT SHOULD RETURN THE SAME RETURN VALUE
			mockPersonsRepository.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

			// ACT
			PersonResponse responseFromAdd = await personService.AddPerson(personAddRequest);

			testOutputHelper.WriteLine(responseFromAdd.ToString());
			personResponse_expected.Id = responseFromAdd.Id;
			//ASSERT

			Assert.True(responseFromAdd.Id != Guid.Empty);
			responseFromAdd.Id.Should().NotBe(Guid.Empty);

			responseFromAdd.Should().Be(personResponse_expected);
		}

		#endregion 

		#region GetPersonById
		[Fact]
		public async Task GetPersonById_NullId_ToBeNull()
		{
			Guid? id = null;


			PersonResponse? resp = await personService.GetPerson(id);


			resp.Should().BeNull();

			Assert.Null(resp);
		}
		[Fact]
		public async Task GetPersonById_ValidId_ToBeSuccesful()
		{
			Person person = GetPerson();
			PersonResponse personResponseAdd = person.ToPersonResponse();

			mockPersonsRepository.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

			//Act
			PersonResponse? personRequestGet =  await personService.GetPerson(person.Id);


			//Assert
			Assert.Equal(personResponseAdd, personRequestGet);
			personResponseAdd.Should().Be(personRequestGet);

		}
		#endregion

		#region GetAllPersons

		[Fact]
		public async Task GetAllPersons_EmptyList()
		{
			//Arrange

			mockPersonsRepository.Setup(teemp => teemp.GetAllPersons()).ReturnsAsync(new List<Person>());

			var listResp = await personService.GetAllPersons();

			listResp.Should().BeEmpty();

			Assert.Empty(listResp);
		}

		[Fact]
		public async Task GetAllPersons_GetListWithFewPersons_ToBeSuccesfull()
		{
			//Arrange
			List<Person> persons = new List<Person>()
			{
				GetPerson(),
				GetPerson(),
				GetPerson(),
				GetPerson(),
			};
			List<PersonResponse> listRespFromAdd_EXPEXCTED = persons.Select(c => c.ToPersonResponse()).ToList();

			//printer
			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd_EXPEXCTED)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			//repository
			mockPersonsRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetAllPersons();

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
			foreach (var person in listRespFromGet)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			listRespFromAdd_EXPEXCTED.Should().BeEquivalentTo(listRespFromGet);

		}

		#endregion

		 #region GetFilteredPersons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchList()
		{
			List<Person> persons = new();

			List<PersonResponse> listRespFromAdd_EXPEXCTED = persons.Select(c => c.ToPersonResponse()).ToList();


			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd_EXPEXCTED)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			mockPersonsRepository.Setup(temp => temp.GetFilteredPerson(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetFilteredPerson(nameof(Person.PersonName), "");

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
			foreach (var person in listRespFromGet)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}
			//Assert
			listRespFromGet.Should().BeEquivalentTo(listRespFromAdd_EXPEXCTED);


		//	listRespFromGet.Should().BeEquivalentTo(personsAddRequests);

		}
		[Fact]
		public async Task GetFilteredPersons_ValidSearchList()
		{
			List<Person> persons = new() 
			{
				GetPerson(),
				GetPerson(),
				GetPerson(),
				GetPerson(),
				GetPerson(),
			};

			List<PersonResponse> listRespFromAdd_EXPEXCTED = persons.Select(c => c.ToPersonResponse()).ToList();


			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd_EXPEXCTED)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			mockPersonsRepository.Setup(temp => temp.GetFilteredPerson(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

			//ACT
			List<PersonResponse> listRespFromGet = await personService.GetFilteredPerson(nameof(Person.PersonName), "sa");

			testOutputHelper.WriteLine("\n" + "	Received List(from get all) : \n");
			foreach (var person in listRespFromGet)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}
			//Assert
			listRespFromGet.Should().BeEquivalentTo(listRespFromAdd_EXPEXCTED);

		}

		#endregion 

		#region GetSortedPersons
		[Fact]
		public async Task GetSortedPersons_ValidData()
		{
			List<Person> persons = new()
			{
				GetPerson(),
				GetPerson(),
				GetPerson(),
				GetPerson(),
				GetPerson(),
			};

			List<PersonResponse> listRespFromAdd_EXPEXCTED = persons.Select(c => c.ToPersonResponse()).ToList();


			testOutputHelper.WriteLine("	Expected List : \n");
			foreach (var person in listRespFromAdd_EXPEXCTED)
			{
				//print person list after add
				testOutputHelper.WriteLine(person.ToString());
			}

			mockPersonsRepository.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

			var allPersons = await personService.GetAllPersons();
			//ACT
			List<PersonResponse> listRespFromSort = await personService
				.GetSortedPersons(allPersons,nameof(Person.PersonName), SortOrderOptions.Descending);

			listRespFromSort.Should().BeInDescendingOrder(teemp => teemp.PersonName);


		}
		#endregion

		#region UpdatePerson

		[Fact]
		public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
		{
			PersonUpdateRequest? personUpdateRequest = null;

			mockPersonsRepository.Setup(temp => temp.UpdatePersonByPerson(It.IsAny<Person>())).ReturnsAsync(null as Person);

			await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			{
				await personService.UpdatePerson(personUpdateRequest);
			});
		}

		[Fact]
		public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
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
		public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
		{
			//Arrange
			#region adding

			Person person = fixture.Build<Person>()
				.With(temp => temp.Email, "qqweewq@mail.ru")
				.With(temp => temp.PersonName, null as string)
				.With(temp => temp.Country, null as Country)
				.With(temp => temp.Gender, GenderOptions.Male.ToString())
				.Create();

			#endregion
			
			PersonUpdateRequest? personUpdateRequest = person.ToPersonResponse().ToPersonUpdateRequest();

			//Act and Assert
			await Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await personService.UpdatePerson(personUpdateRequest);
			});
		}
		[Fact]
		public async Task UpdatePerson_PersonFullDetails()
		{
			Person person = GetPerson();

			PersonResponse personsResponse_expected = person.ToPersonResponse();

			PersonUpdateRequest? personUpdateRequest = personsResponse_expected.ToPersonUpdateRequest();

			mockPersonsRepository.Setup(temp => temp.UpdatePersonByPerson(It.IsAny<Person>())).ReturnsAsync(person);

			mockPersonsRepository.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);


			var responseAfterUpdate = await personService.UpdatePerson(personUpdateRequest);

			Assert.Equal(personsResponse_expected, responseAfterUpdate);
			personsResponse_expected.Should().Be(responseAfterUpdate);
		}

		#endregion

		#region DeletePerson

		[Fact]
		public async Task DeletePerson_ValidPersonId()
		{

			//Arrange 
			Person person = GetPerson();

			mockPersonsRepository.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);

			mockPersonsRepository.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

			//aCT
			bool result = await personService.DeletePerson(person.Id);

			//ASSERT
			Assert.True(result);
		}

		[Fact]
		public async Task DeletePerson_InvalidPersonId_ToBeFalse()
		{

			Person person = GetPerson();

			mockPersonsRepository.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(false);

			mockPersonsRepository.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

			//Arrange & Act
			bool result = await personService.DeletePerson(Guid.NewGuid());

			//ASSERT
			Assert.False(result);
		}

		#endregion

		#region Tool Methods

		private Person GetPerson()
		{
			return fixture.Build<Person>().With(temp => temp.Email, "qqweewq@mail.ru")
				.With(temp => temp.Country, null as Country)
				.With(temp => temp.Gender, GenderOptions.Male.ToString())
				.Create();
		}

		#endregion
	}
}
