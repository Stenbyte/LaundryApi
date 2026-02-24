
namespace TenantApi.Services;

public interface ITenantService
{
    string TestConnection();

    string TestPgConnectionWithDbContext();
}