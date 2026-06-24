using HomePageApp.Core.Interfaces;
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

        public GoogleAuthService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<string?> GetValidAccessTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var expiresAtString = await httpContext.GetTokenAsync("expires_at");
            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(expiresAtString) || string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }

            if (DateTimeOffset.TryParse(expiresAtString, out var expiresAt))
            {
                // Refresh if token is already expired or expiring within 5 minutes
                if (expiresAt < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    var refreshToken = await httpContext.GetTokenAsync("refresh_token");
                    if (string.IsNullOrEmpty(refreshToken)) return accessToken;

                    var newTokens = await RequestNewTokenFromGoogleAsync(refreshToken);
                    if (newTokens != null)
                    {
                        var result = await httpContext.AuthenticateAsync();
                        if (result.Succeeded)
                        {
                            result.Properties.UpdateTokenValue("access_token", newTokens.AccessToken);

                            if (!string.IsNullOrEmpty(newTokens.RefreshToken))
                            {
                                result.Properties.UpdateTokenValue("refresh_token", newTokens.RefreshToken);
                            }

                            var newExpiration = DateTimeOffset.UtcNow.AddSeconds(newTokens.ExpiresIn);
                            result.Properties.UpdateTokenValue("expires_at", newExpiration.ToString("o"));

                            // Write the updated tokens back to the user's encrypted auth cookie
                            await httpContext.SignInAsync(result.Principal, result.Properties);
                            return newTokens.AccessToken;
                        }
                    }
                    else
                    {
                        await httpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                        httpContext.Response.Redirect("/account/login");
                        return null;
                    }
                }
            }

            return accessToken;
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
            var response = await client.PostAsync("https://googleapis.com", requestContent);

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
