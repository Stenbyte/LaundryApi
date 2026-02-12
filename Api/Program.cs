using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using TenantApi.Services;
using TenantApi.Models;
using TenantApi.Validators;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AspNetCoreRateLimit;
using TenantApi.Repository;
using LaundryBooking.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

[assembly: ApiController]

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", optional: true)
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
    if (builder.Environment.IsProduction())
    {
        options.RequireHttpsMetadata = true;
    }
    else
    {
        options.RequireHttpsMetadata = false;
    }

    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = "sub",
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents {
        OnAuthenticationFailed = async ctx => {
            ctx.Response.StatusCode = 401;
            ctx.Response.ContentType = "application/json";


            string message = "Validation failed";
            if (ctx.Response.GetType() == typeof(SecurityTokenExpiredException))
            {
                message = "Token expired";
            }

            var result = JsonSerializer.Serialize(new {
                error = message,
                detail = ctx.Exception.Message
            });
            await ctx.Response.WriteAsync(result);
        },
        OnChallenge = async ctx => {
            ctx.HandleResponse();

            if (!ctx.Response.HasStarted)
            {
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new {
                    error = "Not Authorized",
                    detail = "You must provide token"
                }
                );
                await ctx.Response.WriteAsync(result);
            }

        }
    };
});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));


if (!builder.Environment.IsProduction())
{
    builder.Services.Configure<PostgresSettings>(
        builder.Configuration.GetSection("Postgres")
    );

    builder.Services.AddDbContextPool<TenantDbContext>(options => {
        var settings = builder.Configuration.GetSection("Postgres").Get<PostgresSettings>();

        options.UseNpgsql($"Host={settings?.Host};Port={settings?.Port};Database={settings?.DatabaseName};Username={settings?.UserName};Password={settings?.Password}");
    });
}



builder.Services.AddSingleton(sp => {
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<ILaundryService, LaundryService>();
builder.Services.AddScoped<ILaundryRepository, LaundryRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<JwtService>();

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

    if (!builder.Environment.IsProduction())
    {
        var pgStatus = laundryService.TestPgConnectionWithDbContext();
        Console.WriteLine($"++++++++++🍏🍏🍏${pgStatus}++++++++++++++");
    }
    Console.WriteLine($"++++++++++🍏🍏🍏 Test Connection to MongoDB: ${dataBase}++++++++++++++");
}
catch (Exception ex)
{
    Console.Error.WriteLine("------------🍎🍎🍎 Startup connection test failed ------------");
    Console.Error.WriteLine(ex);
    throw;
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("customPolicy");
app.UseIpRateLimiting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
