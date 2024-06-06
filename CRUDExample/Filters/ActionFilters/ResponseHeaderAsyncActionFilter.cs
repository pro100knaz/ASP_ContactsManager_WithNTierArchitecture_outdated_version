using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
	public class ResponseHeaderAsyncActionFilter : IAsyncActionFilter, IOrderedFilter
	{
		private readonly ILogger<ResponseHeaderAsyncActionFilter> logger;
		private readonly string key;
		private readonly string value;

		public int Order { get; set; }
		public ResponseHeaderAsyncActionFilter(ILogger<ResponseHeaderAsyncActionFilter> logger, string key, string value, int order)
		{
			this.logger = logger;
			this.key = key;
			this.value = value;
			Order = order;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			logger.LogInformation("{FilterName}.{MethodName} method executing before", nameof(ResponseHeaderAsyncActionFilter), nameof(OnActionExecutionAsync));
			
			await next(); //calls filter or action //mandatory like meddleware


			context.HttpContext.Response.Headers[key] = value;
			logger.LogInformation("{FilterName}.{MethodName} method executed ", nameof(ResponseHeaderAsyncActionFilter), nameof(OnActionExecutionAsync));
		}
	}
}
