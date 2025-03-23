using System;
using Student_App.Models;

namespace Student_App.Services.Reports
{
    public class ReportDateValidator
    {
        // Check if reporting is allowed for a specific week
        public static bool IsReportingAllowed(DateTime weekStart, Student student)
        {
            // Convert string dates to DateTime
            if (!TryParseDate(student.join_course_date, out DateTime joinDate))
                return false;
                
            // If exit date is provided, use it for validation
            bool hasExitDate = TryParseDate(student.exit_course_date, out DateTime exitDate);
            
            // Current date (for future date validation)
            DateTime currentDate = DateTime.Now.Date;
            
            // 1. Week must not be before join date
            if (weekStart < joinDate)
                return false;
                
            // 2. Week must not be after exit date (if provided)
            if (hasExitDate && weekStart > exitDate)
                return false;
                
            // 3. Week must not be in the future
            if (weekStart > currentDate)
                return false;
                
            return true;
        }
        
        // Try to parse a date from string format
        private static bool TryParseDate(string dateString, out DateTime result)
        {
            result = DateTime.MinValue;
            
            if (string.IsNullOrEmpty(dateString))
                return false;
                
            // Try various formats (adjust as needed based on API date format)
            string[] formats = {
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss",
                "MM/dd/yyyy"
            };
            
            return DateTime.TryParseExact(
                dateString,
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out result);
        }
        
        // Get the first valid week for reporting
        public static DateTime GetFirstValidWeek(Student student)
        {
            if (TryParseDate(student.join_course_date, out DateTime joinDate))
            {
                // Find the start of the week containing join date
                int diff = (7 + (joinDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                return joinDate.AddDays(-1 * diff);
            }
            
            // Default to current week if join date not available
            DateTime now = DateTime.Now.Date;
            int currentDiff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * currentDiff);
        }
        
        // Get the last valid week for reporting
        public static DateTime GetLastValidWeek(Student student)
        {
            // Try to get exit date
            if (TryParseDate(student.exit_course_date, out DateTime exitDate))
            {
                // Find the start of the week containing exit date
                int diff = (7 + (exitDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                return exitDate.AddDays(-1 * diff);
            }
            
            // If no exit date, use current date
            DateTime now = DateTime.Now.Date;
            int currentDiff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * currentDiff);
        }
        
        // Check if date is within student's enrollment period
        public static bool IsDateWithinEnrollment(DateTime date, Student student)
        {
            // Try to parse join date
            if (!TryParseDate(student.join_course_date, out DateTime joinDate))
                return false;
                
            // Check join date
            if (date < joinDate)
                return false;
                
            // If exit date exists, check that too
            if (TryParseDate(student.exit_course_date, out DateTime exitDate))
            {
                if (date > exitDate)
                    return false;
            }
            
            return true;
        }
    }
} 