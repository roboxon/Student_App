using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Student_App.Models;

namespace Student_App.Services.Reports
{
    public class LocalReportStorageManager
    {
        private static string BaseStoragePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentApp",
            "WeekReports");

        // Save a weekly report to local storage
        public static async Task<bool> SaveWeekReportAsync(DateTime weekStart, WeeklyReport report)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(BaseStoragePath);

                // Get file path
                string filePath = GetStorageFilePath(weekStart);

                // Serialize and save
                string json = JsonConvert.SerializeObject(report, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
                
                // Invalidate any cached status for this week
                ReportStatusCalculator.InvalidateCache(weekStart);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get a weekly report from local storage
        public static async Task<WeeklyReport> GetWeekReportAsync(DateTime weekStart)
        {
            string filePath = GetStorageFilePath(weekStart);
            
            if (!File.Exists(filePath))
                return null;
                
            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<WeeklyReport>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Remove local report file after successful sync
        public static void RemoveAfterSync(DateTime weekStart)
        {
            try
            {
                string filePath = GetStorageFilePath(weekStart);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                
                // Invalidate cached status
                ReportStatusCalculator.InvalidateCache(weekStart);
            }
            catch (Exception)
            {
                // Ignore errors in cleanup
            }
        }

        // Get the file path for a specific week
        public static string GetStorageFilePath(DateTime weekStart)
        {
            return Path.Combine(
                BaseStoragePath,
                $"WeekReport_{weekStart:yyyy-MM-dd}.json");
        }

        // Check if a report exists for a specific week
        public static bool ReportExists(DateTime weekStart)
        {
            return File.Exists(GetStorageFilePath(weekStart));
        }
    }
} 