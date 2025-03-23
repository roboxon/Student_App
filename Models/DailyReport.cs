using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Student_App.Models
{
    public class DailyReport
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("primarySubjectId")]
        public int PrimarySubjectId { get; set; }

        [JsonProperty("primaryTopicId")]
        public int PrimaryTopicId { get; set; }

        [JsonProperty("hourlyReports")]
        public List<HourlyReport> HourlyReports { get; set; } = new List<HourlyReport>();

        [JsonProperty("hoursReported")]
        public decimal HoursReported { get; set; }

        [JsonProperty("dailySummary")]
        public string DailySummary { get; set; }

        // Calculate hours reported
        public void UpdateHoursReported()
        {
            HoursReported = 0;
            foreach (var report in HourlyReports)
            {
                if (!string.IsNullOrEmpty(report.LearningDescription))
                {
                    // Calculate hours between start and end time
                    if (TimeSpan.TryParse(report.StartTime, out TimeSpan start) && 
                        TimeSpan.TryParse(report.EndTime, out TimeSpan end))
                    {
                        var duration = end - start;
                        HoursReported += (decimal)duration.TotalHours;
                    }
                }
            }
        }
    }
} 