using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using Student_App.Services.Interfaces;

namespace Student_App.Services
{
    public class TokenService : ITokenService
    {
        private static TokenService? _instance;
        private static readonly object _lock = new object();

        private readonly HttpClient _httpClient;
        private readonly string _tokenEndpoint;
        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _tokenExpiry;

        public static TokenService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new TokenService();
                    }
                }
                return _instance;
            }
        }

        private TokenService()
        {
            _httpClient = new HttpClient();
            _tokenEndpoint = "https://example.com/token"; // Replace with actual token endpoint
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new
                {
                    username = username,
                    password = password
                };

                var response = await _httpClient.PostAsync(
                    _tokenEndpoint,
                    new StringContent(
                        JsonSerializer.Serialize(loginData),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var tokens = JsonSerializer.Deserialize<TokenResponse>(tokenResponse);
                    
                    if (tokens != null)
                    {
                        _accessToken = tokens.AccessToken;
                        _refreshToken = tokens.RefreshToken;
                        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
                throw new Exception("Not authenticated");

            if (DateTime.UtcNow >= _tokenExpiry)
            {
                await RefreshTokenAsync();
            }

            return _accessToken ?? throw new Exception("Access token is null");
        }

        private async Task RefreshTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_refreshToken))
                    throw new Exception("No refresh token available");

                var refreshData = new
                {
                    refreshToken = _refreshToken
                };

                var response = await _httpClient.PostAsync(
                    $"{_tokenEndpoint}/refresh",
                    new StringContent(
                        JsonSerializer.Serialize(refreshData),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    var tokens = JsonSerializer.Deserialize<TokenResponse>(tokenResponse);
                    
                    if (tokens != null)
                    {
                        _accessToken = tokens.AccessToken;
                        _refreshToken = tokens.RefreshToken;
                        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn);
                    }
                }
                else
                {
                    throw new Exception("Token refresh failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Token refresh failed: {ex.Message}");
            }
        }

        public void Logout()
        {
            _accessToken = null;
            _refreshToken = null;
            _tokenExpiry = DateTime.MinValue;
        }

        public void StoreTokens(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
        }

        public string? GetAccessToken() => _accessToken;
        public string? GetRefreshToken() => _refreshToken;

        public void ClearTokens()
        {
            _accessToken = null;
            _refreshToken = null;
        }

        public bool HasValidTokens()
        {
            return !string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_refreshToken);
        }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
    }
} 