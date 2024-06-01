using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Enities
{
	public class PersonsDbContext : DbContext
	{
        public PersonsDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Person> Persons { get; set;}
        public DbSet<Country> Countries { get; set;}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//To Rename Table
			modelBuilder.Entity<Country>().ToTable("Countries");
			modelBuilder.Entity<Person>().ToTable("Persons");

			string countriesJson = File.ReadAllText("countries.json");

			var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson)!;

				modelBuilder.Entity<Country>().HasData(countries.ToArray());

			string personsJson = File.ReadAllText("persons.json");

			var persons = JsonSerializer.Deserialize<List<Person>>(personsJson)!;

			foreach (Person person in persons)
			{
				modelBuilder.Entity<Person>().HasData(person);
			}


			modelBuilder.Entity<Person>().Property(temp => temp.TIN)
				.HasColumnName("TaxIdentificationNumber")
				.HasColumnType("varchar(8)")
				.HasDefaultValue("ABC12345");
			//modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();

			modelBuilder.Entity<Person>().ToTable(t => t.HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8"));


			modelBuilder.Entity<Person>(entity =>
			{
				entity.HasOne<Country>(c => c.Country)
				.WithMany(p => p.Persons)
				.HasForeignKey(p => p.CountryId);
			}); //Необязательно

		}

		public IQueryable<Person> sp_GetAllPersons()
		{
			return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]");
		}

		public int sp_InsertPerson(Person person)
		{
			SqlParameter[] sqlParameters = new SqlParameter[]
			{
				new SqlParameter("@Id", person.Id),
				new SqlParameter("@PersonName", person.PersonName),
				new SqlParameter("@Email", person.Email),
				new SqlParameter("@DateOfBirth", person.DateOfBirth),
				new SqlParameter("@Gender", person.Gender),
				new SqlParameter("@CountryId", person.CountryId),
				new SqlParameter("@Address", person.Address),
				new SqlParameter("@ReceiveNewsLatters", person.ReceiveNewsLatters),
			};

			/*
			 
			 @Id uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40), @DateOfBirth datetime2(7), 
@Gender varchar(10), @CountryId uniqueidentifier, @Address nvarchar(200), @ReceiveNewsLatters bit)
        AS BEGIN
          INSERT INTO [dbo].[Persons](Id, PersonName, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsLatters) VALUES (@Id, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLatters)
        END
			 
			  public int sp_InsertPerson(Person person)
    {
      SqlParameter[] parameters = new SqlParameter[] { 
        new SqlParameter("@PersonID", person.PersonID),
        new SqlParameter("@PersonName", person.PersonName),
        new SqlParameter("@Email", person.Email),
        new SqlParameter("@DateOfBirth", person.DateOfBirth),
        new SqlParameter("@Gender", person.Gender),
        new SqlParameter("@CountryID", person.CountryID),
        new SqlParameter("@Address", person.Address),
        new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
      };

      return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
    }


			 */
			return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @Id, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLatters ", sqlParameters);

		}

    }
}
