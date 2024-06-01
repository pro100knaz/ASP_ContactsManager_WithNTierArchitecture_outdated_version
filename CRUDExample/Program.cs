using Enities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICountriesService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();

 
builder.Services.AddDbContext<PersonsDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("PersonDb")); //явное укаазание тспользоание типа бд
});

var app = builder.Build();


if(builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

 
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();


app.Run(); 
