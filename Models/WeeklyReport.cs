using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Student_App.Models
{
    public class WeeklyReport
    {
        [JsonProperty("studentId")]
        public int StudentId { get; set; }

        [JsonProperty("weekNumber")]
        public int WeekNumber { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }

        [JsonProperty("dailyReports")]
        public List<DailyReport> DailyReports { get; set; } = new List<DailyReport>();

        [JsonProperty("totalHoursReported")]
        public decimal TotalHoursReported { get; set; }

        [JsonProperty("subjectsCovered")]
        public List<int> SubjectsCovered { get; set; } = new List<int>();

        [JsonProperty("weeklySummary")]
        public string WeeklySummary { get; set; }

        [JsonProperty("isComplete")]
        public bool IsComplete { get; set; }

        // Initialize daily reports for the week
        public void InitializeWeek(Student student)
        {
            // Initialize DailyReports if it's null
            if (DailyReports == null)
                DailyReports = new List<DailyReport>();
            else
                DailyReports.Clear();
            
            // Handle null student
            if (student == null || student.working_days == null)
            {
                // Create a basic week with empty days
                for (int i = 0; i < 7; i++)
                {
                    DateTime currentDate = StartDate.AddDays(i);
                    DailyReports.Add(new DailyReport
                    {
                        Date = currentDate,
                        HourlyReports = new List<HourlyReport>()
                    });
                }
                return;
            }
            
            // Get working days from student (safely)
            var workingDayNumbers = student.working_days
                .Where(wd => wd != null)
                .Select(wd => (int)wd.day_number)
                .ToList();
            
            // Create reports for each day of the week (that is a working day)
            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = StartDate.AddDays(i);
                int dayOfWeek = (int)currentDate.DayOfWeek;
                
                // Adjust for week starting with Monday in some systems
                if (dayOfWeek == 0) dayOfWeek = 7; // Sunday becomes 7
                
                // Create a daily report for this day (whether it's a working day or not)
                var dailyReport = new DailyReport
                {
                    Date = currentDate,
                    HourlyReports = new List<HourlyReport>()
                };

                // Only add hour slots if it's a working day
                if (workingDayNumbers.Contains(dayOfWeek))
                {
                    // Add hour slots based on student's schedule
                    var daySchedule = student.working_days.FirstOrDefault(wd => wd != null && (int)wd.day_number == dayOfWeek);
                    if (daySchedule != null)
                    {
                        // Parse start and end times
                        if (!string.IsNullOrEmpty(daySchedule.start_time) && 
                            !string.IsNullOrEmpty(daySchedule.end_time) && 
                            TimeSpan.TryParse(daySchedule.start_time, out TimeSpan startTime) && 
                            TimeSpan.TryParse(daySchedule.end_time, out TimeSpan endTime))
                        {
                            // Create hour slots (assuming 1-hour increments)
                            for (TimeSpan time = startTime; time < endTime; time = time.Add(TimeSpan.FromHours(1)))
                            {
                                string hourStart = time.ToString(@"hh\:mm");
                                string hourEnd = time.Add(TimeSpan.FromHours(1)).ToString(@"hh\:mm");
                                
                                dailyReport.HourlyReports.Add(new HourlyReport
                                {
                                    StartTime = hourStart,
                                    EndTime = hourEnd,
                                    LastUpdated = DateTime.Now
                                });
                            }
                        }
                    }
                }

                DailyReports.Add(dailyReport);
            }
        }

        // Update weekly totals
        public void UpdateTotals()
        {
            TotalHoursReported = 0;
            SubjectsCovered.Clear();
            
            foreach (var daily in DailyReports)
            {
                daily.UpdateHoursReported();
                TotalHoursReported += daily.HoursReported;
                
                // Collect unique subjects
                foreach (var hourly in daily.HourlyReports)
                {
                    if (hourly.SubjectId > 0 && !SubjectsCovered.Contains(hourly.SubjectId))
                    {
                        SubjectsCovered.Add(hourly.SubjectId);
                    }
                }
            }
        }

        // Submit report to server
        public async Task<bool> SubmitToServerAsync(string token)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set up headers
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    client.DefaultRequestHeaders.Add("week_date", StartDate.ToString("yyyy-MM-dd"));

                    // Serialize the report
                    string jsonContent = JsonConvert.SerializeObject(this);
                    StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Send to server
                    string endpoint = $"{AppConfig.ApiBaseUrl}/weeklyReport/upload";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    // Handle response
                    if (response.IsSuccessStatusCode)
                    {
                        // Mark as submitted
                        foreach (var daily in DailyReports)
                        {
                            foreach (var hourly in daily.HourlyReports)
                            {
                                hourly.IsSubmitted = true;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        // Could add error handling here
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Fetch report from server if it exists
        public static async Task<WeeklyReport> FetchFromServerAsync(int studentId, DateTime weekStart, string token)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Set up headers
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    // Calculate week number and year
                    int weekNumber = GetIso8601WeekOfYear(weekStart);
                    int year = weekStart.Year;

                    // Send request
                    string endpoint = $"{AppConfig.ApiBaseUrl}/weeklyReport/{year}/{weekNumber}";
                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    // Handle response
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<WeeklyReport>(jsonResponse);
                    }
                    else
                    {
                        // Report doesn't exist or error
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Helper to get ISO8601 week number
        private static int GetIso8601WeekOfYear(DateTime date)
        {
            System.Globalization.CultureInfo cultureInfo = 
                System.Globalization.CultureInfo.CurrentCulture;
            return cultureInfo.Calendar.GetWeekOfYear(
                date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
    }
} 