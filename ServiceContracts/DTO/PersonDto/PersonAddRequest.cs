using Enities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO.PersonDto
{
	public class PersonAddRequest
	{
		[Required(ErrorMessage = "Person Name can't be blank ")]
		public string? PersonName { get; set; }
		[Required(ErrorMessage = "Email can't be blank ")]
		[EmailAddress(ErrorMessage ="INVALID EMAIL")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		public DateTime? DateOfBirth { get; set; }
		[Required(ErrorMessage = "Please select a gender")]
		public GenderOptions? Gender { get; set; }
		[Required(ErrorMessage = "Please select a countryy, it can't be blank ")]
		public Guid? CountryId { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLatters { get; set; }


		public Person ToPerson() => new Person
		{
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
