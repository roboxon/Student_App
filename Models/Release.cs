using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Student_App.Models
{
    public class Release
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string API_BASE_URL = "https://learnpath.elexbo.de";
        
        // Properties matching API response
        public int id { get; set; }
        public int program_id { get; set; }
        public int company_id { get; set; }
        public string? created_at { get; set; }
        public int created_by { get; set; }
        public int version { get; set; }
        public int sub_version { get; set; }
        public string? archived_at { get; set; }
        public int? archived_by { get; set; }
        public ReleaseContent? content { get; set; }
        public string? release_name { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        
        // Get local storage path for release data
        private static string GetLocalStoragePath(int releaseId)
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Student_App"
            );
            
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
                
            return Path.Combine(directory, $"release_{releaseId}.json");
        }
        
        // Check if local release exists and matches current release ID
        public static bool IsLocalReleaseValid(int releaseId)
        {
            string filePath = GetLocalStoragePath(releaseId);
            if (!File.Exists(filePath))
                return false;
                
            try
            {
                string json = File.ReadAllText(filePath);
                var releaseResponse = JsonConvert.DeserializeObject<ReleaseApiResponse>(json);
                return releaseResponse?.data?.id == releaseId;
            }
            catch
            {
                return false;
            }
        }
        
        // Save release to local storage
        private static void SaveReleaseToLocal(ReleaseApiResponse response, int releaseId)
        {
            try
            {
                string filePath = GetLocalStoragePath(releaseId);
                string json = JsonConvert.SerializeObject(response, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Just log the error, don't throw - we can still function without local storage
                System.Diagnostics.Debug.WriteLine($"Error saving release to local storage: {ex.Message}");
            }
        }
        
        // Load release from local storage
        private static Release? LoadReleaseFromLocal(int releaseId)
        {
            try
            {
                string filePath = GetLocalStoragePath(releaseId);
                if (!File.Exists(filePath))
                    return null;
                    
                string json = File.ReadAllText(filePath);
                var response = JsonConvert.DeserializeObject<ReleaseApiResponse>(json);
                return response?.data;
            }
            catch
            {
                return null;
            }
        }
        
        // Get release data - implements the caching logic you described
        public static async Task<Release> GetReleaseAsync(Student? student, bool forceRefresh = false)
        {
            if (student == null || string.IsNullOrEmpty(student.access_token))
                throw new ArgumentException("Valid student with access token is required");
                
            int releaseId = student.release_id;
            
            // Try to load from local cache if valid and not forced to refresh
            if (!forceRefresh && IsLocalReleaseValid(releaseId))
            {
                var cachedRelease = LoadReleaseFromLocal(releaseId);
                if (cachedRelease != null)
                    return cachedRelease;
            }
            
            // If we get here, we need to fetch from API
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {student.access_token}");
            
            string endpoint = $"{API_BASE_URL}/release/read/?id={releaseId}";
            
            var response = await _client.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException($"Failed to fetch release data: {response.StatusCode}");
            }
            
            var releaseResponse = JsonConvert.DeserializeObject<ReleaseApiResponse>(responseContent);
            if (releaseResponse == null || releaseResponse.response_code != 200)
            {
                throw new ApiException(releaseResponse?.service_message ?? "Invalid response format");
            }
            
            // Save the response to local storage for future use
            SaveReleaseToLocal(releaseResponse, releaseId);
            
            return releaseResponse.data;
        }
    }
    
    public class ReleaseApiResponse
    {
        public int response_code { get; set; }
        public string? message { get; set; }
        public int count { get; set; }
        public string? service_message { get; set; }
        public Release? data { get; set; }
    }
    
    public class ReleaseContent
    {
        public Program? program { get; set; }
        public List<SubjectWithTopics>? subjects { get; set; }
        public int max_priority { get; set; }
    }
    
    public class Program
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string? program_name { get; set; }
        public string? created_at { get; set; }
        public string? program_description { get; set; }
        public string? program_tag { get; set; }
        public bool is_active { get; set; }
        public int program_hours { get; set; }
        public int program_version { get; set; }
    }
    
    public class SubjectWithTopics
    {
        public Subject? subject { get; set; }
        public List<TopicWithLessons>? topics { get; set; } = new List<TopicWithLessons>();
    }
    
    public class Subject
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string? subject_name { get; set; }
        public string? subject_description { get; set; }
        public int subject_hours { get; set; }
        public string? subject_tag { get; set; }
        public string? subject_version { get; set; }
        public int priority { get; set; }
    }
    
    public class TopicWithLessons
    {
        public Topic? topic { get; set; }
        public List<Lesson>? lessons { get; set; } = new List<Lesson>();
        public List<Resource>? resources { get; set; } = new List<Resource>();
    }
    
    public class Topic
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int subject_id { get; set; }
        public string? topic_name { get; set; }
        public int topic_hours { get; set; }
        public string? topic_description { get; set; }
        public string? topic_tag { get; set; }
        public int priority { get; set; }
    }
    
    public class Lesson
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int topic_id { get; set; }
        public string? lesson_name { get; set; }
        public string? lesson_description { get; set; }
        public string? lesson_tag { get; set; }
        public int lesson_hours { get; set; }
        public int priority { get; set; }
    }
    
    public class Resource
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string? topic_id { get; set; }
        public int creator_id { get; set; }
        public string? creator_name { get; set; }
        public string? last_update { get; set; }
        public string? description { get; set; }
        public string? url_link { get; set; }
    }
} 