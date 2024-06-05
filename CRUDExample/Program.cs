using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



//Logging Configuration
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();a
//builder.Logging.AddDebug();
//builder.Logging.AddEventLog();


//Serilog
//using var log = new LoggerConfiguration().CreateLogger();
Log.Logger = new LoggerConfiguration().CreateLogger();
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration logger) =>
{
	logger.ReadFrom.Configuration(context.Configuration) //read configuration settigs from buil-in IConfiguration
	.ReadFrom.Services(services); // read out current app's services and make thrm available to serilog
});

//builder.Services.AddHttpLogging(configureOptions =>
//{
//	//need to work app.UseHttpLogging(); even if configureOptions lambda is null it is still working and idk why but let it be
//	configureOptions.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties 
//	| Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders; //in the same way we can add more or less information inside logging fields

//});

builder.Services.AddControllersWithViews();


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



if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}


//app.UseHttpLogging();
app.UseSerilogRequestLogging();

if (!builder.Environment.IsEnvironment("Test"))
{
	Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Logger.LogCritical("Critical");
app.Logger.LogError("Error");
app.Logger.LogWarning("Warning");
app.Logger.LogInformation("Information");

app.Run();


public partial class Program { } // make the auto-generated Program accessible programmatically
								 //to make it available in our code