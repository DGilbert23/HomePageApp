using HomePageApp.Core.Interfaces.Google;
using HomePageApp.Core.Interfaces.Identity;
using HomePageApp.Infrastructure.Services.Google.GoogleAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HomePageApp.Infrastructure.Services.Google.GoogleAuth
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUserAccountService _userAccountService;
        private readonly GoogleTokenProtector _tokenProtector;

        public GoogleAuthService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IUserAccountService userAccountService,
            GoogleTokenProtector tokenProtector)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _userAccountService = userAccountService;
            _tokenProtector = tokenProtector;
        }

        public async Task<string?> GetValidAccessTokenAsync()
        {
            var googleConnection =
                await _userAccountService.GetGoogleConnectionAsync();

            if (googleConnection == null)
                return null;


            var refreshToken =
                _tokenProtector.Unprotect(
                    googleConnection.EncryptedRefreshToken);


            var newTokens =
                await RequestNewTokenFromGoogleAsync(refreshToken);


            if (newTokens == null)
                return null;


            return newTokens.AccessToken;
        }

        private async Task<GoogleTokenResponse?> RequestNewTokenFromGoogleAsync(string refreshToken)
        {
            using var client = new HttpClient();
            var tokenRequestParams = new Dictionary<string, string>
            {
                { "client_id", _configuration["Authentication:Google:ClientId"] ?? string.Empty },
                { "client_secret", _configuration["Authentication:Google:ClientSecret"] ?? string.Empty },
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParams);
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", requestContent);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
        }

        public async Task<GoogleAuthDto?> GetRemoteDataAsync(CancellationToken cancellationToken = default)
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

            return await _httpClient.GetFromJsonAsync<GoogleAuthDto>("v1/protected-records", cancellationToken);
        }
    }

    internal class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
