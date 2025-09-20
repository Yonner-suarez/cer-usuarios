using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using microUsuarios.API.Model.Request.Validators;
using microUsuarios.API.Utils;
using microUsuarios.API.Utils.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n del entorno y variables
var env = builder.Environment;
var configuration = builder.Configuration;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

Variables.env = $"appsettings.{env.EnvironmentName}.json";
Console.WriteLine("Ambiente: " + Variables.env);
if (env.IsProduction())
{
    Variables.env = "appsettings.json";
}

// --------------------------------------
// Servicios
// --------------------------------------

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json" });
});

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(AgregarUsuarioValidator).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(IniciarSesionValidator).Assembly);

// Controladores + Filtros
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
    options.Filters.Add<UnhandledExceptionFilterAttribute>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// JWT Authentication
var key = Encoding.ASCII.GetBytes(Variables.Token.Bearer);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Microservicio Usuarios"
    });
});

// --------------------------------------
// Build & Middleware
// --------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.0.1");
});

app.UseRouting();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
