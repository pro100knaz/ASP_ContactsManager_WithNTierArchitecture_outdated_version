using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using Services;
using Serilog;
using CRUDExample.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);



//Logging Configuration
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();a
//builder.Logging.AddDebug();
//builder.Logging.AddEventLog();


//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

	loggerConfiguration
	.ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
	.ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

//adds controllers and views as services
builder.Services.AddControllersWithViews(options =>
{
	//Adding Global Filters

	//options.Filters.Add<ResponseHeaderActionFilter>(); // no parameters (onlu order)

	var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>(); //to build the required services
																								 //GetService return null
																								 //GetRequiredService throw exception, but if need for sure it is bettter ofc


	options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global", 0));  //With Parameters )


	var logger1 = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderAsyncActionFilter>>();
	options.Filters.Add(new ResponseHeaderAsyncActionFilter(logger1, "My-Key-From-Global-Async", "My-Value-From-Global-Async", 0));  //With Parameters )

});

builder.Services.AddHttpLogging(configureOptions =>
{
	//need to work app.UseHttpLogging(); even if configureOptions lambda is null it is still working and idk why but let it be
	configureOptions.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
	| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders; //in the same way we can add more or less information inside logging fields

});



builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("PersonDb")); //явное укаазание тспользоание типа бд
	//.LogTo(Console.WriteLine, LogLevel.Information); //he LogTo() method takes an Action as a parameter, that specifies where log messages should be sent,
													 //and a LogLevel value that specifies the minimum log level at which messages should be sent.
});








var app = builder.Build();


app.UseSerilogRequestLogging();


if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}


app.UseHttpLogging();


if (!builder.Environment.IsEnvironment("Test"))
{
	Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();



app.Run();


public partial class Program { } // make the auto-generated Program accessible programmatically
								 //to make it available in our code