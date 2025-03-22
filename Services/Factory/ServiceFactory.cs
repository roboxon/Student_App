using Student_App.Services.Interfaces;
using System;

namespace Student_App.Services.Factory
{
    public class ServiceFactory
    {
        private static ServiceFactory? _instance;
        private static readonly object _lock = new object();

        private ServiceFactory() { }

        public static ServiceFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ServiceFactory();
                    }
                }
                return _instance;
            }
        }

        public ITokenService GetTokenService()
        {
            return TokenService.Instance;
        }

        public IApiService GetApiService()
        {
            return new ApiService(GetTokenService());
        }
    }
} 