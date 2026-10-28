using Services;
using ServiceContract;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Connections;
using OfficeOpenXml;
using RepositoryContracts;
using Repositories;
ExcelPackage.License.SetNonCommercialPersonal("Abdelrahman Farag");
var builder = WebApplication.CreateBuilder(args);

#region Services
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30

#endregion

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
if (builder.Environment.IsEnvironment("Test") == false)
{
  Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
public partial class Program { }