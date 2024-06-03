using CsvHelper;
using Enities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using RepositoryContracts.interfaces;
using RepositoriesImplementation;

namespace Services
{
	public class PersonService : IPersonService
	{
		private readonly IPersonsRepository personsRepository;

		public PersonService(IPersonsRepository personsRepository)
		{
			this.personsRepository = personsRepository;
		}

		public async Task<PersonResponse> AddPerson(PersonAddRequest? addPerson)
		{
			if (addPerson is null)
			{
				throw new ArgumentNullException(nameof(addPerson));
			}

			ValidationHelper.ModelValidation(addPerson);

			var person = addPerson.ToPerson();
			person.Id = Guid.NewGuid();

			await personsRepository.AddPerson(person);

			return person.ToPersonResponse() ?? throw new Exception();
		}

		public async Task<List<PersonResponse>> GetAllPersons()
		{
			//List<PersonResponse> personResponses = new List<PersonResponse>();
			var persons = await personsRepository.GetAllPersons();
			var personResponses = persons.Select(c => ((c).ToPersonResponse())).ToList();

			//var x = DbContext.sp_GetAllPersons().ToList();
			//{
			//	var res = new PersonResponse();
			//	res = c.ToPersonResponse();
			//	return c.ToPersonResponse();
			//}

			return personResponses ?? new List<PersonResponse>();
		}

		public async Task<PersonResponse?> GetPerson(Guid? id)
		{

			return (id) == null ? null : (await personsRepository.GetPersonById(id.Value))?.ToPersonResponse();
		}

		public async Task<List<PersonResponse>> GetFilteredPerson(string searchBy, string? searchString)
		{
			//if(searchBy == nameof(PersonResponse.DateOfBirth))
			//{
			//	if (DateTime.TryParseExact(searchString, "dd MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime searchDate))
			//	{
			//		return (await personsRepository.GetFilteredPerson(temp =>
			//			temp.DateOfBirth.HasValue &&
			//			temp.DateOfBirth.Value.Date == searchDate.Date)).Select(temp => temp.ToPersonResponse()).ToList();
			//	}
			//}

			List<Person> persons = searchBy switch
			{
				nameof(PersonResponse.PersonName) =>
				(await personsRepository.GetFilteredPerson(temp =>
					temp.PersonName.Contains(searchString))),

				nameof(PersonResponse.Email) =>
				(await personsRepository.GetFilteredPerson(temp =>
					temp.Email.Contains(searchString))),

 

				nameof(PersonResponse.Gender) =>
					(await personsRepository.GetFilteredPerson(temp =>
						temp.Gender.Contains(searchString))),


				nameof(PersonResponse.Country) =>
					(await personsRepository.GetFilteredPerson(temp =>
						temp.Country.Name.Contains(searchString))),

				nameof(PersonResponse.Address) =>
			(await personsRepository.GetFilteredPerson(temp =>
				temp.Address.Contains(searchString))),

				_ => await personsRepository.GetAllPersons()
			};
			return persons.Select(temp => temp.ToPersonResponse()).ToList();
		}

		public async  Task<List<PersonResponse>> GetSortedPersons
			(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrderOptions)
		{
			if (string.IsNullOrEmpty(sortBy))
			{
				return allPersons;
			}
			List<PersonResponse> result = (sortBy, sortOrderOptions) switch
			{
				(nameof(PersonResponse.PersonName), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.PersonName), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),


				(nameof(PersonResponse.Email), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Email), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

				(nameof(PersonResponse.DateOfBirth), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),



				(nameof(PersonResponse.Age), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Age).ToList(),

				(nameof(PersonResponse.Age), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Age).ToList(),



				(nameof(PersonResponse.Gender), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Gender), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),




				(nameof(PersonResponse.Address), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Address), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),


				(nameof(PersonResponse.Country), SortOrderOptions.Ascending)
			=> allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

				(nameof(PersonResponse.Country), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),



				(nameof(PersonResponse.ReceiveNewsLatters), SortOrderOptions.Ascending)
				=> allPersons.OrderBy(temp => temp.ReceiveNewsLatters).ToList(),

				(nameof(PersonResponse.ReceiveNewsLatters), SortOrderOptions.Descending)
				=> allPersons.OrderByDescending(temp => temp.ReceiveNewsLatters).ToList(),

				_ => allPersons
			};

			return result;

		}

		public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? updatePerson)
		{
			if (updatePerson is null)
				throw new ArgumentNullException(nameof(updatePerson));

			ValidationHelper.ModelValidation(updatePerson);

			Person? matchingPerson = await personsRepository.GetPersonById(updatePerson.PersonId);

			if (matchingPerson is null)
			{
				throw new ArgumentException("Given person id does not exist");
			}

			//update all deetails

			matchingPerson.PersonName = updatePerson.PersonName;
			matchingPerson.Gender = updatePerson.Gender.ToString();
			matchingPerson.Address = updatePerson.Address;
			matchingPerson.CountryId = updatePerson.CountryId;
			matchingPerson.DateOfBirth = updatePerson.DateOfBirth;
			matchingPerson.Email = updatePerson.Email;
			matchingPerson.ReceiveNewsLatters = updatePerson.ReceiveNewsLatters;

			await personsRepository.UpdatePersonByPerson(matchingPerson);

			return (matchingPerson).ToPersonResponse() ?? throw new Exception();
		}

		public async Task<bool> DeletePerson(Guid? id)
		{
			if (id == null)
			{
				throw new ArgumentNullException(nameof(id));
			}

			var x = await GetPerson(id);
			if (x is null)
				return false;

			Person? resultPerson = await personsRepository.GetPersonById(id.Value);

			if (resultPerson == null)
				return false;

			var result = await personsRepository.DeletePersonByPersonId(id.Value);

			return result;
		}

		public async Task<MemoryStream> GetPersonCSV()
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);

			CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

			csvWriter.WriteHeader<PersonResponse>();

			csvWriter.NextRecord();

			var persons = await GetAllPersons();

			await csvWriter.WriteRecordsAsync(persons);

			memoryStream.Position = 0;

			return memoryStream;

		}

		public async Task<MemoryStream> GetPersonExcel()
		{
			MemoryStream memoryStream = new MemoryStream(); //хранит что угодно
			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet1");
				worksheet.Cells["A1"].Value = "Person Name";
				worksheet.Cells["B1"].Value = "Email";
				worksheet.Cells["C1"].Value = "Date of Birth";
				worksheet.Cells["D1"].Value = "Age";
				worksheet.Cells["E1"].Value = "Gender";
				worksheet.Cells["F1"].Value = "Country";
				worksheet.Cells["G1"].Value = "Address";
				worksheet.Cells["H1"].Value = "Receive New Latters";

				using (ExcelRange headersCells = worksheet.Cells["A1:H1"])
				{
					headersCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.LightVertical;
					headersCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.BlueViolet);
					headersCells.Style.Font.Bold = true;
				}

				int row = 2;

				var persons = await GetAllPersons();

				foreach (PersonResponse person in persons)
				{
					worksheet.Cells[row, 1].Value = person.PersonName;
					worksheet.Cells[row, 2].Value = person.Email;
					if (person.DateOfBirth.HasValue)
						worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
					worksheet.Cells[row, 4].Value = person.Age;
					worksheet.Cells[row, 5].Value = person.Gender;
					worksheet.Cells[row, 6].Value = person.Country;
					worksheet.Cells[row, 7].Value = person.Address;
					worksheet.Cells[row, 8].Value = person.ReceiveNewsLatters;
					row++;
				}

				worksheet.Cells[$"A1:H{row}"].AutoFitColumns(); //auto size of the data

				await excelPackage.SaveAsync();
			}

			memoryStream.Position = 0;
			return memoryStream;
		}
	}
}
