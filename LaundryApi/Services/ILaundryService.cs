
namespace LaundryApi.Services;

public interface ILaundryService 
{
    string TestConnection();

    string TestPgConnectionWithDbContext();
}