using System;
using System.Threading.Tasks;
using Student_App.Services.Interfaces;
using Student_App.Services.Configuration;

namespace Student_App.Services.Base
{
    public abstract class BaseApiService : IApiService
    {
        protected readonly ITokenService _tokenService;
        protected readonly string _baseUrl;

        protected BaseApiService(ITokenService tokenService, string? baseUrl = null)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _baseUrl = baseUrl ?? AppConfig.ApiBaseUrl;
        }

        public virtual async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            try
            {
                var token = await _tokenService.GetAccessTokenAsync();
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"{_baseUrl}/{endpoint}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<T>(content);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"API Error: {ex.Message}");
            }
        }

        public virtual async Task<T?> PostAsync<T>(string endpoint, object data) where T : class
        {
            try
            {
                var token = await _tokenService.GetAccessTokenAsync();
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var json = System.Text.Json.JsonSerializer.Serialize(data);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{_baseUrl}/{endpoint}", content);
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<T>(responseContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"API Error: {ex.Message}");
            }
        }
    }
} 