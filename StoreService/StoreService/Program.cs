using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
  .AddOpenIddict()
  .AddClient(options =>
  {
    options.AllowPasswordFlow()
    .AllowAuthorizationCodeFlow();

    options.DisableTokenStorage();

    options.AddDevelopmentEncryptionCertificate()
    .AddDevelopmentSigningCertificate();

    options.UseSystemIntegration();

    options.UseSystemNetHttp();

    options.AddRegistration(new OpenIddictClientRegistration
    {
      Issuer = new Uri("https://localhost:10001/", UriKind.Absolute),

      ClientId = "store-client-id",
      RedirectUri = new Uri("/", UriKind.Relative),
      Scopes = { "store-api" }
    });
  });

builder.Services.AddCors();

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors(b => b.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();