using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using LaundryApi.Services;
using LaundryApi.Models;
using LaundryApi.Validators;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreRateLimit;
using LaundryApi.Repository;
using LaundryBooking.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Template.json", optional: true)
    .AddEnvironmentVariables();

var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins");

builder.Services.AddCors(options => {
    options.AddPolicy(name: "customPolicy", policy => {
        policy.WithOrigins(allowedOrigins!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
}
);




var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));


builder.Services.Configure<PostgresSettings>(
    builder.Configuration.GetSection("Postgres")
);

builder.Services.AddSingleton(sp => {
    var settings = sp.GetRequiredService<IOptions<PostgresSettings>>().Value;
    var connectionString = $"Host={settings.Host};Port={settings.Port};Database={settings.DatabaseName};Username={settings.UserName};Password={settings.Password}";

    var connection = new Npgsql.NpgsqlConnection(connectionString);
    return connection;
});

builder.Services.AddSingleton(sp => {
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<ILaundryService, LaundryService>();
builder.Services.AddScoped<ILaundryRepository, LaundryRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSingleton<JwtService>();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters().AddValidatorsFromAssemblyContaining<SignUpValidator>();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddAuthorization();

if (builder.Environment.IsProduction())
{
    builder.WebHost.ConfigureKestrel(options => {
        options.ListenAnyIP(int.Parse("8080"));
    });
}

var app = builder.Build();



var scope = app.Services.CreateScope();
var laundryService = scope.ServiceProvider.GetRequiredService<ILaundryService>();
try
{
    var dataBase = laundryService.TestConnection();
    var pgStatus = laundryService.TestPgConnection();
    Console.WriteLine($"++++++++++🍏🍏🍏 Test Connection to MongoDB: ${dataBase}++++++++++++++");
    Console.WriteLine($"++++++++++🍏🍏🍏${pgStatus}++++++++++++++");
}
catch (Exception ex)
{
    throw new Exception($"------------🍎🍎🍎 Test Connection failed: ${ex}------------");
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseCors("customPolicy");
app.UseIpRateLimiting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
