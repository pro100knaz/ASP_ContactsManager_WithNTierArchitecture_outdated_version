using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> logger;

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            this.logger = logger;
        }
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //To Do Before
            logger.LogInformation("Inside {FilterName}. {MethodName} method executing before", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));


			context.HttpContext.Response.Headers["MyModification"] = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
			//context.HttpContext.Response.Headers.Add("MyModification", value: DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
			context.HttpContext.Response.Headers["awda"] = "adwawdawdawdawdawd";
			context.HttpContext.Response.Headers["awdawd"] = "adwawdawdawdawdawd";
			context.HttpContext.Response.Headers["awdadw"] = "adwawdawdawdawdawd";

			await next();
            logger.LogInformation("Inside {FilterName}. {MethodName} method  executed", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            //To do After
          
        }
    }
}
