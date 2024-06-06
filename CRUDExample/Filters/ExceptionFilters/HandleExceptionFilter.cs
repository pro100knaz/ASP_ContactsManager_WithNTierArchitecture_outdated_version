using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ExceptionFilters
{
	public class HandleExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<HandleExceptionFilter> logger;

		public HandleExceptionFilter(ILogger<HandleExceptionFilter> logger)
        {
			this.logger = logger;
		}
        public void OnException(ExceptionContext context)
		{
			//only when the exception is raised
			logger.LogError("Inside {FilterName} {FilterName} , \n {ExceptionType} \n {ExceptionMethod}", 
				nameof(HandleExceptionFilter), nameof(OnException),
				context.Exception.GetType().ToString(),
				context.Exception.Message);

			context.Result = new ContentResult()
			{
				Content = context.Exception.Message,
				StatusCode = 500,
			};
		}
	}
}
