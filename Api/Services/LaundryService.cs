using TenantApi.Exceptions;
using TenantApi.Repository;


namespace TenantApi.Services
{
    public class LaundryService : ILaundryService
    {
        private readonly ITenantRepository _repository;

        public LaundryService(ITenantRepository repository)
        {
            _repository = repository;
        }

        public string TestConnection()
        {
            try
            {
                return _repository.TestConnection();
            }
            catch (CustomException ex)
            {
                throw new CustomException("DataBase connection failed", ex, 500);
            }
        }

        public string TestPgConnectionWithDbContext()
        {
            try
            {
                return _repository.TestPgConnectionWithDbContext();
            }
            catch (CustomException ex)
            {
                throw new CustomException("DataBase connection failed", ex, 500);
            }
        }
    }
}