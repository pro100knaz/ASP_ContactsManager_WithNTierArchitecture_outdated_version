using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO.PersonDto;

namespace CRUDExample.Filters.ActionFilters
{
	public class PersonCreateAndEditPostCreateActionFilter : IAsyncActionFilter
	{
		private readonly ILogger<PersonCreateAndEditPostCreateActionFilter> logger;
		private readonly ICountriesService countriesService;

		public PersonCreateAndEditPostCreateActionFilter(ILogger<PersonCreateAndEditPostCreateActionFilter> logger, ICountriesService countriesService)
		{
			this.logger = logger;
			this.countriesService = countriesService;
		}
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			//TO DO BEFORE
			if (context.Controller is PersonsController personsController)
			{
				if (!context.ModelState.IsValid)
				{
					var countries = await countriesService.GetAllCountrise();
					personsController.ViewBag.Countries = countries.Select(temp => new SelectListItem()
					{
						Text = temp.CountryName,
						Value = temp.CountryId.ToString()
					});

					personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

					var personRequest = context.ActionArguments["personRequest"];
					context.Result = personsController.View(personRequest);  //subsequence wiil stop when Result will become not null //short-circuiting
				}
				else
				{
					await next(); //вызывает последующие фильтры и методы
					//TO DO AFTER
				}
			}


		}
	}
}
