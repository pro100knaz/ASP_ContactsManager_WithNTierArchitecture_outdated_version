using Enities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO.PersonDto
{
	public class PersonResponse
	{
		public override string ToString()
		{
			return $" Name = {PersonName}, \n email = {Email}, \n date of birthday = {DateOfBirth} \n CountryId = {CountryId}";
		}
		public Guid Id { get; set; }
		public string? PersonName { get; set; }
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public double? Age { get; set; }
		public string? Gender { get; set; }
		public string? Country { get; set; }
		public Guid? CountryId { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLatters { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is null)
				return false;

			if (obj is PersonResponse person)
			{
				return Id == person.Id;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return Id.GetHashCode();

		}


		public PersonUpdateRequest ToPersonUpdateRequest()
		{
			return new PersonUpdateRequest()
			{
				PersonId = Id,
				PersonName = PersonName,
				Email = Email,
				Address = Address,
				CountryId = CountryId,
				DateOfBirth = DateOfBirth,
				Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender ?? "Other", true),
				ReceiveNewsLatters = ReceiveNewsLatters,
			};
		}

	}
	public static class PersonExtension
	{
		public static PersonResponse ToPersonResponse(this Person person)
		{
			return new PersonResponse
			{
				PersonName = person.PersonName,
				Id = person.Id,
				Email = person.Email,
				DateOfBirth = person.DateOfBirth,
				Gender = person.Gender,
				Country = person.Country?.Name,
				CountryId = person.CountryId,
				Address = person.Address,
				ReceiveNewsLatters = person.ReceiveNewsLatters,
				Age = person.DateOfBirth == null ? null : Math.Round(((DateTime.Now - person.DateOfBirth.Value).TotalDays) / 356.25)
			};
		}

		//public static PersonResponse ConvertPersonToPersonResponse(Person? person)
		//{
		//	if (person == null)
		//		return new PersonResponse();

		//	PersonResponse personResponse = person.ToPersonResponse();
		//	personResponse.Country = person.Country?.Name;
		//	return personResponse;
		//}


	}

}
