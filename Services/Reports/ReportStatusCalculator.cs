using System;
using System.Collections.Generic;
using Student_App.Models;

namespace Student_App.Services.Reports
{
    public enum ReportStatus
    {
        None,       // No reports created
        Partial,    // Some hours have reports
        Complete    // All working hours have reports
    }

    public class ReportStatusCalculator
    {
        // Cache for status calculations to improve performance
        private static Dictionary<string, ReportStatus> _statusCache = new Dictionary<string, ReportStatus>();

        // Calculate the status of a weekly report
        public static ReportStatus CalculateWeekStatus(WeeklyReport report, Student student)
        {
            if (report == null)
                return ReportStatus.None;

            // Check if we have a cached result
            string cacheKey = $"{report.StudentId}_{report.StartDate:yyyyMMdd}";
            if (_statusCache.ContainsKey(cacheKey))
                return _statusCache[cacheKey];

            // Calculate required hours for the week based on student schedule
            decimal requiredHours = GetRequiredHoursForWeek(report.StartDate, student);
            
            // Get actual reported hours from report data
            decimal reportedHours = GetReportedHoursFromData(report);

            // Determine status
            ReportStatus status;
            if (reportedHours == 0)
                status = ReportStatus.None;
            else if (reportedHours < requiredHours)
                status = ReportStatus.Partial;
            else
                status = ReportStatus.Complete;

            // Cache the result
            _statusCache[cacheKey] = status;
            
            return status;
        }

        // Calculate required hours for a week based on student schedule
        public static decimal GetRequiredHoursForWeek(DateTime weekStart, Student student)
        {
            decimal totalHours = 0;

            // Loop through 7 days of the week
            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = weekStart.AddDays(i);
                int dayOfWeek = (int)currentDate.DayOfWeek;
                
                // Adjust for week starting with Monday in some systems
                if (dayOfWeek == 0) dayOfWeek = 7; // Sunday becomes 7
                
                // Find matching working day in student schedule
                var workingDay = student.working_days.Find(wd => (int)wd.day_number == dayOfWeek);
                if (workingDay != null)
                {
                    // Calculate hours for this day
                    if (TimeSpan.TryParse(workingDay.start_time, out TimeSpan startTime) && 
                        TimeSpan.TryParse(workingDay.end_time, out TimeSpan endTime))
                    {
                        TimeSpan duration = endTime - startTime;
                        totalHours += (decimal)duration.TotalHours;
                    }
                }
            }

            return totalHours;
        }

        // Get the total reported hours from a report
        public static decimal GetReportedHoursFromData(WeeklyReport report)
        {
            decimal totalHours = 0;

            foreach (var daily in report.DailyReports)
            {
                foreach (var hourly in daily.HourlyReports)
                {
                    if (!string.IsNullOrEmpty(hourly.LearningDescription))
                    {
                        // Calculate hours between start and end time
                        if (TimeSpan.TryParse(hourly.StartTime, out TimeSpan startTime) && 
                            TimeSpan.TryParse(hourly.EndTime, out TimeSpan endTime))
                        {
                            TimeSpan duration = endTime - startTime;
                            totalHours += (decimal)duration.TotalHours;
                        }
                    }
                }
            }

            return totalHours;
        }

        // Clear the cache for a specific week or all weeks
        public static void InvalidateCache(DateTime? weekStart = null)
        {
            if (weekStart.HasValue)
            {
                // Clear cache for specific week
                string cacheKey = $"*_{weekStart.Value:yyyyMMdd}";
                var keysToRemove = new List<string>();
                
                foreach (var key in _statusCache.Keys)
                {
                    if (key.EndsWith(weekStart.Value.ToString("yyyyMMdd")))
                        keysToRemove.Add(key);
                }
                
                foreach (var key in keysToRemove)
                {
                    _statusCache.Remove(key);
                }
            }
            else
            {
                // Clear entire cache
                _statusCache.Clear();
            }
        }
    }
} 