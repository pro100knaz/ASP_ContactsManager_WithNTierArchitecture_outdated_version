using Enities;
using Microsoft.EntityFrameworkCore;
using RepositoriesImplementation;
using RepositoryContracts.interfaces;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();

 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("PersonDb")); //явное укаазание тспользоание типа бд
});

var app = builder.Build();


if(builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath:"Rotativa");
 
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run(); 
