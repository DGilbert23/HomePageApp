using HomePageApp.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HomePageApp.Infrastructure.Services.Google.GoogleAuth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ExternalDataDto?> GetRemoteDataAsync(CancellationToken cancellationToken = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                return null;
            }

            return await _httpClient.GetFromJsonAsync<ExternalDataDto>("v1/protected-records", cancellationToken);
        }
    }
}
