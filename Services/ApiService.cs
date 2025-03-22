using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Student_App.Services.Interfaces;
using Student_App.Services.Base;

namespace Student_App.Services
{
    public class ApiService : BaseApiService
    {
        public ApiService(ITokenService tokenService, string? baseUrl = null)
            : base(tokenService, baseUrl)
        {
        }

        public override Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            return base.GetAsync<T>(endpoint);
        }

        public override Task<T?> PostAsync<T>(string endpoint, object data) where T : class
        {
            return base.PostAsync<T>(endpoint, data);
        }
    }
} 