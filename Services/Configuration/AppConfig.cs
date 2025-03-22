using System;
using System.Configuration;

namespace Student_App.Services.Configuration
{
    public static class AppConfig
    {
        // Base URLs for different environments
        public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"] ?? "https://api.example.com/auth";
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://api.example.com";
        public static int TokenRefreshThreshold => int.Parse(ConfigurationManager.AppSettings["TokenRefreshThreshold"] ?? "300");
        
        // Service-specific configurations
        public static class Services
        {
            public static class Reports
            {
                public static string BaseUrl => ConfigurationManager.AppSettings["ReportsServiceUrl"] ?? $"{ApiBaseUrl}/reports";
                public static string Endpoint => ConfigurationManager.AppSettings["ReportsEndpoint"] ?? "reports";
                public static int Timeout => int.Parse(ConfigurationManager.AppSettings["ReportsTimeout"] ?? "30");
            }

            public static class Attendance
            {
                public static string BaseUrl => ConfigurationManager.AppSettings["AttendanceServiceUrl"] ?? $"{ApiBaseUrl}/attendance";
                public static string Endpoint => ConfigurationManager.AppSettings["AttendanceEndpoint"] ?? "attendance";
                public static int Timeout => int.Parse(ConfigurationManager.AppSettings["AttendanceTimeout"] ?? "30");
            }

            public static class Schedule
            {
                public static string BaseUrl => ConfigurationManager.AppSettings["ScheduleServiceUrl"] ?? $"{ApiBaseUrl}/schedule";
                public static string Endpoint => ConfigurationManager.AppSettings["ScheduleEndpoint"] ?? "schedule";
                public static int Timeout => int.Parse(ConfigurationManager.AppSettings["ScheduleTimeout"] ?? "30");
            }

            public static class Profile
            {
                public static string BaseUrl => ConfigurationManager.AppSettings["ProfileServiceUrl"] ?? $"{ApiBaseUrl}/profile";
                public static string Endpoint => ConfigurationManager.AppSettings["ProfileEndpoint"] ?? "profile";
                public static int Timeout => int.Parse(ConfigurationManager.AppSettings["ProfileTimeout"] ?? "30");
            }
        }

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