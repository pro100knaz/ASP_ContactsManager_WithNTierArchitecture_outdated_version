using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
	public class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
	{
		private readonly ILogger<ResponseHeaderActionFilter> logger;
		private readonly string key;
		private readonly string value;

		public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
		{
			this.logger = logger;
			this.key = key;
			this.value = value;
			Order = order;
		}

		public int Order { get; set; }

		public void OnActionExecuted(ActionExecutedContext context)
		{
			logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));
			context.HttpContext.Response.Headers[key] = value;
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{
			logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));

		}
	}
}
