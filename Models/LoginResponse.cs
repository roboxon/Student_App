using System.Collections.Generic;

namespace Student_App.Models
{
    public class LoginResponse
    {
        public int response_code { get; set; }
        public string? message { get; set; }
        public int count { get; set; }
        public string? service_message { get; set; }
        public LoginData? data { get; set; }

        public bool IsSuccessful()
        {
            return response_code == 200 && data != null;
        }

        public string GetErrorMessage()
        {
            if (!string.IsNullOrEmpty(service_message))
            {
                return service_message;
            }
            if (!string.IsNullOrEmpty(message))
            {
                return message;
            }
            return "An unknown error occurred during login.";
        }
    }

    public class LoginData
    {
        public Student? student { get; set; }
        public List<WorkingDay>? days { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
    }
} 