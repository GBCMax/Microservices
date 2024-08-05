using AuthorizationServer.Apps;
using AuthorizationServer.Database;
using AuthorizationServer.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Globalization;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)));

  options.UseOpenIddict();
});

builder.Services.AddIdentity<User, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

builder.Services
  .AddOpenIddict()
  .AddCore(options =>
  {
    options.UseEntityFrameworkCore()
    .UseDbContext<ApplicationDbContext>();
  })
  .AddServer(options =>
  {
    options
    .SetAuthorizationEndpointUris("connect/authorize")
    .SetTokenEndpointUris("connect/token")
    .SetIntrospectionEndpointUris("connect/introspect");

    options.AllowPasswordFlow();

    //options.AddEncryptionKey(new SymmetricSecurityKey(
    //        Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

    options.AcceptAnonymousClients();

    options.AddDevelopmentEncryptionCertificate()
    .AddDevelopmentSigningCertificate();

    options.UseAspNetCore()
    .EnableAuthorizationEndpointPassthrough()
    .EnableTokenEndpointPassthrough();
  })
  .AddValidation(options =>
  {
    options.UseLocalServer();

    options.UseAspNetCore();
  });

builder.Services.AddCors();

builder.Services.AddAuthorization();

builder.Services.AddHostedService<Seeder>();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultControllerRoute();

app.Run();