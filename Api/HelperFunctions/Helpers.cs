using Microsoft.Extensions.Caching.Memory;

namespace TenantApi.Helpers
{
    public class HelperFunctions
    {
        public void TrackFailedAttempt(string email, IMemoryCache _cache)
        {

            string failedEmail = $"failed_{email}";

            int attempts = _cache.GetOrCreate(failedEmail, entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(11);
                return 0;
            });

            attempts++;

            if (attempts >= 5)
            {
                _cache.Set($"lockout_{email}", DateTime.UtcNow.AddMinutes(10));
                _cache.Remove(failedEmail);
            }
            else
            {
                _cache.Set(failedEmail, attempts, TimeSpan.FromMinutes(15));
            }

        }

        public void ResetFailedAttempts(string email, IMemoryCache _cache)
        {
            _cache.Remove($"lockout_{email}");
            _cache.Remove($"failed_{email}");
        }
    }
}