using Laundry.Services;
using LaundryBooking.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy(name: "customPolicy", policy => {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
}
);


builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<LaundryService>();

builder.Services.AddControllers();

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
app.UseAuthorization();
app.MapControllers();
app.Run();
