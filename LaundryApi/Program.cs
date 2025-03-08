using FluentValidation;
using FluentValidation.AspNetCore;
using LaundryApi.Services;
using LaundryApi.Models;
using LaundryApi.Validators;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy(name: "customPolicy", policy => {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
}
);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes("Secret");

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
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<LaundryService>();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters().AddValidatorsFromAssemblyContaining<SignUpValidator>();

builder.Services.AddAuthorization();

var app = builder.Build();

var scope = app.Services.CreateScope();
var laundryService = scope.ServiceProvider.GetRequiredService<LaundryService>();
try
{
    var dataBase = laundryService.TestConnection();
    Console.WriteLine($"++++++++++🍏 Connected to MongoDB: ${dataBase}++++++++++++++");
}
catch (Exception ex)
{
    throw new Exception($"------------🍎 MongoDB Connection failed: ${ex}------------");
}

// Todo enable it for production
// app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("customPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
