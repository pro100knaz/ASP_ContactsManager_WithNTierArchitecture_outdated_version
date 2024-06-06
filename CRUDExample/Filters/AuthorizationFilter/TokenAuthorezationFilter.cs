using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.AuthorizationFilter
{
    public class TokenAuthorezationFilter : IAuthorizationFilter
    {
        private readonly ILogger<TokenAuthorezationFilter> logger;

        public TokenAuthorezationFilter(ILogger<TokenAuthorezationFilter> logger)
        {
            this.logger = logger;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }
            if(context.HttpContext.Request.Cookies["Auth-Key"] != "A100") //JUST FOR TESTING
            {
				context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
				return;
			}
			if (context.HttpContext.Request.Cookies["Auth-Key"] == "A100") //JUST FOR TESTING
			{
				//
			}
		}
    }
}
