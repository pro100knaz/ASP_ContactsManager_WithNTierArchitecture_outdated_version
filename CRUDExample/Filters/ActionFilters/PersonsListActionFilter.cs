using CRUDExample.Controllers;
using Enities;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using ServiceContracts.DTO.PersonDto;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
	public class PersonsListActionFilter : IActionFilter
	{
		private readonly ILogger<PersonsListActionFilter> _logger;

		public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
		{
			_logger = logger;
		}
		public void OnActionExecuted(ActionExecutedContext context)
		{
			//To do: add after logic here
			_logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));


			//Add Info To View Data
			PersonsController personsController = (PersonsController)context.Controller;

			IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
			if (parameters is not null)
			{
				if (parameters.ContainsKey("searchBy"))
				{
					var temp = Convert.ToString(parameters["searchBy"]);
					personsController.ViewData["CurrentSearchBy"] =  Convert.ToString(parameters["searchBy"]);
				}
				if (parameters.ContainsKey("searchString"))
				{
					var temp = Convert.ToString(parameters["searchString"]);
					personsController.ViewData["CurrentSearchString"] = Convert.ToString( parameters["searchString"]);
				}

				if (parameters.ContainsKey("sortBy"))
				{
					var temp = Convert.ToString(parameters["sortBy"]);
					personsController.ViewData["CurrentSortBy"] = Convert.ToString( parameters["sortBy"]);
				}
				else
				{
					personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
				}
				if (parameters.ContainsKey("sortOrder"))
				{
					var temp = Convert.ToString(parameters["sortOrder"]);
					personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
				}
				else
				{
					personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.Ascending);
				}
			}

			personsController.ViewBag.SearchFields = new Dictionary<string, string>()
			{
				{ nameof(PersonResponse.PersonName), "Person Name"},
				{ nameof(PersonResponse.Email), "Email"},
				{ nameof(PersonResponse.Gender), "Gender"},
				{ nameof(PersonResponse.CountryId), "CountryId"},
				{ nameof(PersonResponse.Address), "Address"},
			};
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			//To do: add before logic here
			_logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));


			context.HttpContext.Items["arguments"] = context.ActionArguments;

			//Validation
			if (context.ActionArguments.ContainsKey("searchBy"))
			{
				string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
				if (!string.IsNullOrEmpty(searchBy))
				{
					var searchByOptions = new List<string>()
					{
						nameof(PersonResponse.PersonName),
						nameof(PersonResponse.Email),
						nameof(PersonResponse.Gender),
						nameof(PersonResponse.CountryId),
						nameof(PersonResponse.Address), 
					};

					if (searchByOptions.Any(temp => temp == searchBy) == false)
					{
						_logger.LogInformation("searchBy actual value {0}", searchBy);
						context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
						_logger.LogInformation("searchBy updated value {0}", nameof(PersonResponse.PersonName));
					}
				}
			}
		}
	}
}
