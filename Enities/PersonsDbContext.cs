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

			var countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);

				modelBuilder.Entity<Country>().HasData(countries.ToArray());

			string personsJson = File.ReadAllText("persons.json");

			var persons = JsonSerializer.Deserialize<List<Person>>(personsJson);

			foreach (Person person in persons)
			{
				modelBuilder.Entity<Person>().HasData(person);
			}

		}
    }
}
