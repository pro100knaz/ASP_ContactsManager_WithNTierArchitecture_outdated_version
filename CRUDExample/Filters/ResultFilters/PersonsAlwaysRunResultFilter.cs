using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CRUDExample.Filters.ResultFilters
{
	public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
	{
		public void OnResultExecuted(ResultExecutedContext context)
		{
			//is possible ONLY if class SkipFilter has Interface IFilterMetadata!
			if (context.Filters.OfType<SkipFilter>().Any()) // just for example of potential of scipping filters that are suppliyed globally or for class. not for a method.
			{
				return;
			}
			//Something that will pe appliyed always

		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			if(context.Filters.OfType<SkipFilter>().Any()) // just for example of potential of scipping filters that are suppliyed globally or for class. not for a method.
			{
				return;
			}
			//Something that will pe appliyed always


		}
	}
}
