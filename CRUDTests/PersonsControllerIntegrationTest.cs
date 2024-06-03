using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
	public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
			_client = factory.CreateClient();

        }

        #region Index

        [Fact]
		public async Task Index_ShoulReturnView()
		{
			//Arrange

			//act
			HttpResponseMessage response = await _client.GetAsync("/Persons/Index");
			//Assert
			response.Should().BeSuccessful();

			var responseBody = await response.Content.ReadAsStringAsync();

			HtmlDocument html = new HtmlDocument();

			html.LoadHtml(responseBody);

			var DOCUMENT = html.DocumentNode;

			DOCUMENT.QuerySelectorAll("table.persons").Should().NotBeNull(); //tag table class persons
		}
		#endregion
	}
}
