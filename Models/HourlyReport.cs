using System;
using Newtonsoft.Json;

namespace Student_App.Models
{
    public class HourlyReport
    {
        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("subjectId")]
        public int SubjectId { get; set; }

        [JsonProperty("topicId")]
        public int TopicId { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("topicName")]
        public string TopicName { get; set; }

        [JsonProperty("learningDescription")]
        public string LearningDescription { get; set; }

        [JsonProperty("isSubmitted")]
        public bool IsSubmitted { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public HourlyReport()
        {
            LastUpdated = DateTime.Now;
            IsSubmitted = false;
        }

        // Display time range (for UI)
        [JsonIgnore]
        public string TimeRange => $"{StartTime}-{EndTime}";
    }
} 