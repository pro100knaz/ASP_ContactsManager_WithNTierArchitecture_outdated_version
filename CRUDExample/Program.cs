using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using Services;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample;

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

builder.Services.ConfigureServices(builder.Configuration);

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