using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using StoreResource.Data;
using StoreResource.Interfaces.IRepo;
using StoreResource.Interfaces.IService;
using StoreResource.Repos;
using StoreResource.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StoreDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(StoreDbContext)));
});

builder.Services.AddTransient<ICarsRepository, CarsRepository>();

builder.Services.AddTransient<ICarsService, CarsService>();

builder.Services.AddOpenIddict()
  .AddValidation(options =>
  {
    options.SetIssuer("https://localhost:10001/");

    options.AddAudiences("store-resource");

    options.UseIntrospection()
    .SetClientId("store-resource")
    .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");

    options.UseSystemNetHttp();

    options.UseAspNetCore();
  });

builder.Services.AddCors();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();