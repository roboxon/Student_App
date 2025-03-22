using System;
using System.Configuration;

namespace Student_App.Services.Configuration
{
    public static class AppConfig
    {
        public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"] ?? "https://api.example.com/auth";
        public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://api.example.com";
        public static int TokenRefreshThreshold => int.Parse(ConfigurationManager.AppSettings["TokenRefreshThreshold"] ?? "300");
        
        public static class Endpoints
        {
            public static string Reports => ConfigurationManager.AppSettings["ReportsEndpoint"] ?? "reports";
            public static string Attendance => ConfigurationManager.AppSettings["AttendanceEndpoint"] ?? "attendance";
            public static string Schedule => ConfigurationManager.AppSettings["ScheduleEndpoint"] ?? "schedule";
            public static string Profile => ConfigurationManager.AppSettings["ProfileEndpoint"] ?? "profile";
        }
    }
} 