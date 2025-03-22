using System.Configuration;

namespace Student_App
{
    public static class AppConfig
    {
        public static class Environment
        {
            public static string Current => ConfigurationManager.AppSettings["Environment"] ?? "Development";
            public static bool IsDevelopment => Current.Equals("Development", StringComparison.OrdinalIgnoreCase);
            public static bool IsProduction => Current.Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"] ?? "https://training.elexbo.de/studentLogin/loginByemailPassword";
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://training.elexbo.de";
        public static int TokenRefreshThreshold => int.Parse(ConfigurationManager.AppSettings["TokenRefreshThreshold"] ?? "300");
    }
} 