using Enities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO.PersonDto
{
	public class PersonUpdateRequest // часть св в могут не обновляться
	{
		[Required(ErrorMessage = "Person Id is Required")]
		public Guid PersonId { get; set; }

		[Required(ErrorMessage = "Person Name can't be blank ")]
		public string? PersonName { get; set; } 
		[Required(ErrorMessage = "Email can't be blank ")]
		[EmailAddress(ErrorMessage = "INVALID EMAIL")]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public GenderOptions? Gender { get; set; }
		public Guid? CountryId { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLatters { get; set; }


		public Person ToPerson() => new Person
		{
			Id = PersonId,
			PersonName = this.PersonName,
			Address = this.Address,
			Email = this.Email,
			DateOfBirth = this.DateOfBirth,
			CountryId = this.CountryId,
			ReceiveNewsLatters = this.ReceiveNewsLatters,
			Gender = this.Gender.ToString(),
		};
	}
}
