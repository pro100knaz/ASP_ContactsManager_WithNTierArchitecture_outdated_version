using CRUDExample.Filters.ActionFilters;
using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using Services;

namespace CRUDExample
{
	public static class ConfigureServiceExtensions
	{

		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{

			//adds controllers and views as services
			services.AddControllersWithViews(options =>
			{
				//Adding Global Filters

				//options.Filters.Add<ResponseHeaderActionFilter>(); // no parameters (onlu order)

				var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>(); //to build the required services
																																//GetService return null
																																//GetRequiredService throw exception, but if need for sure it is bettter ofc


				options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global", 0));  //With Parameters )


				var logger1 = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderAsyncActionFilter>>();
				options.Filters.Add(new ResponseHeaderAsyncActionFilter(logger1, "My-Key-From-Global-Async", "My-Value-From-Global-Async", 0));  //With Parameters )

			});

			services.AddHttpLogging(configureOptions =>
			{
				//need to work app.UseHttpLogging(); even if configureOptions lambda is null it is still working and idk why but let it be
				configureOptions.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
				| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders; //in the same way we can add more or less information inside logging fields

			});
			services.AddScoped<ICountriesRepository, CountriesRepository>();
			services.AddScoped<IPersonsRepository, PersonsRepository>();

			services.AddScoped<ICountriesService, CountryService>();
			services.AddScoped<IPersonService, PersonService>();


			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("PersonDb")); //явное укаазание тспользоание типа бд
																			 //and a LogLevel value that specifies the minimum log level at which messages should be sent.
			});

			return services;
		} 

	}
}
