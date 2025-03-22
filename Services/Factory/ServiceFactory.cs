using Student_App.Services.Interfaces;
using Student_App.Services.Configuration;

namespace Student_App.Services.Factory
{
    public static class ServiceFactory
    {
        private static ITokenService? _tokenService;
        private static IApiService? _apiService;

        public static ITokenService GetTokenService()
        {
            if (_tokenService == null)
            {
                _tokenService = new TokenService(AppConfig.TokenEndpoint);
            }
            return _tokenService;
        }

        public static IApiService GetApiService()
        {
            if (_apiService == null)
            {
                _apiService = new ApiService(GetTokenService(), AppConfig.ApiBaseUrl);
            }
            return _apiService;
        }

        public static void ResetServices()
        {
            _tokenService = null;
            _apiService = null;
        }
    }
} 