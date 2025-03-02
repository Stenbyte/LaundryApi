using Laundry.Services;
using LaundryBooking.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
