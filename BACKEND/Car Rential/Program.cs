using Car_Rential;
using Car_Rential.Authentication;
using Car_Rential.Authorization;
using Car_Rential.Entieties;
using Car_Rential.Helpers;
using Car_Rential.Interfaces;
using Car_Rential.Middleware;
using Car_Rential.Model;
using Car_Rential.Model.Validators;
using Car_Rential.Services;
using Car_Rential.Sieve;
using EuvicIntern.Authentication;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Sieve.Services;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidation();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        }
    );
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Host.UseNLog();
builder.Services.AddDbContext<RentalDbContext>(configuration =>
{
    configuration.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});

var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("JWTInfo").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = authenticationSettings.JwtIssuer,
            ValidAudience = authenticationSettings.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)
            ),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", x => x.RequireClaim(ClaimTypes.NameIdentifier, "1"));
});

builder.Services.AddSingleton<IObjectModelValidator, CustomObjectModelValidator>();
builder.Services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
builder.Services.AddScoped<ICustomersService, CustomersService>();
builder.Services.AddScoped<ICarsService, CarsService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<ICustomerContextService, CustomerContextService>();
builder.Services.AddScoped<ISieveProcessor, AplicationSieveProcessor>();
builder.Services.AddScoped<IValidator<InputCustomerDto>, RegisterCustomerValidator>();
builder.Services.AddScoped<IValidator<InputCustomerDto>, UpdateCustomerValidator>();
builder.Services.AddScoped<IValidator<Reservation>, SeedReservationValidator>();
builder.Services.AddScoped<IValidator<InputCarDto>, RegisterCarValidator>();
builder.Services.AddScoped<IValidator<InputCarDto>, UpdateCarValidator>();
builder.Services.AddScoped<IValidator<ReservationInput>, ReservationInputValidator>();
builder.Services.AddScoped<IAuthorizationHandler, OwnAccountActionHandler>();
builder.Services.AddScoped<CustomersSeeder>();
builder.Services.AddScoped<RegisterCustomerValidator>();
builder.Services.AddScoped<UpdateCustomerValidator>();
builder.Services.AddScoped<UpdateCarValidator>();
builder.Services.AddScoped<RegisterCarValidator>();
builder.Services.AddDirectoryBrowser();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Front",
        builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .WithExposedHeaders("Location");
        }
    );
});

var app = builder.Build();
app.UseStaticFiles();
app.UseCors("Front");
app.UseDirectoryBrowser();
app.UseMiddleware<ErrorHandlingMiddleware>();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<CustomersSeeder>();

seeder.Seeder(10);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registration Task");
});

// Configure the HTTP request pipeline.

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
