using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
	public class TokenResultFilters : IResultFilter
	{
		private readonly ILogger<TokenResultFilters> logger;

		public TokenResultFilters(ILogger<TokenResultFilters> logger)
        {
			this.logger = logger;
		}
        public void OnResultExecuted(ResultExecutedContext context)
		{ 
		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			context.HttpContext.Response.Cookies.Append("Auth-Key", "A100");
			context.HttpContext.Response.Headers["DAWAWDA"] = "ADWDAW";

		}
	}
}
