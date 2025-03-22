using System;
using System.Configuration;
using Student_App.Services.Configuration;

namespace Student_App.Services.Configuration
{
    public static class AppConfig
    {
        // Base URLs and authentication
        public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"] ?? "https://training.elexbo.de/studentLogin/loginByemailPassword";
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://training.elexbo.de";
        public static int TokenRefreshThreshold => int.Parse(ConfigurationManager.AppSettings["TokenRefreshThreshold"] ?? "300");
        
        // Environment-specific settings
        public static class Environment
        {
            public static string Current => ConfigurationManager.AppSettings["Environment"] ?? "Development";
            public static bool IsDevelopment => Current.Equals("Development", StringComparison.OrdinalIgnoreCase);
            public static bool IsStaging => Current.Equals("Staging", StringComparison.OrdinalIgnoreCase);
            public static bool IsProduction => Current.Equals("Production", StringComparison.OrdinalIgnoreCase);
        }
    }
} 