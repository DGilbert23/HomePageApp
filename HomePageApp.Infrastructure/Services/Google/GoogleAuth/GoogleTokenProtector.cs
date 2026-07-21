using Microsoft.AspNetCore.DataProtection;

namespace HomePageApp.Infrastructure.Services.Google.GoogleAuth
{
    public class GoogleTokenProtector
    {
        private readonly IDataProtector _protector;

        public GoogleTokenProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("GoogleCalendarRefreshTokens");
        }

        public string Protect(string value)
        {
            return _protector.Protect(value);
        }

        public string Unprotect(string value)
        {
            return _protector.Unprotect(value);
        }
    }
}