using System.Collections.Generic;
using Newtonsoft.Json;

namespace Student_App.Models
{
    public class Student
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int course_id { get; set; }
        public float? grade_score { get; set; }
        public string? group_name { get; set; }
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? register_at { get; set; }
        public int register_by { get; set; }
        public int? supervisor_id { get; set; }
        public int? mentor_id { get; set; }
        public string? mentor_name { get; set; }
        public string? advisor_id { get; set; }
        public string? advisor_name { get; set; }
        public string? join_course_date { get; set; }
        public string? exit_course_date { get; set; }
        public bool? graduate { get; set; }
        public int course_plan_id { get; set; }
        public int branch_id { get; set; }
        public int release_id { get; set; }
        public int program_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? plan_name { get; set; }
        public string? tag { get; set; }
        public int is_active { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public List<WorkingDay> working_days { get; set; } = new List<WorkingDay>();

        public Student() { }

        public Student(string jsonResponse)
        {
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);
            
            if (loginResponse?.data?.student == null)
            {
                throw new JsonException("Invalid student data in response");
            }

            var student = loginResponse.data.student;
            
            // Assign non-nullable properties
            id = student.id;
            company_id = student.company_id;
            course_id = student.course_id;
            register_by = student.register_by;
            course_plan_id = student.course_plan_id;
            branch_id = student.branch_id;
            release_id = student.release_id;
            program_id = student.program_id;
            is_active = student.is_active;

            // Assign nullable properties
            grade_score = student.grade_score;
            group_name = student.group_name;
            email = student.email;
            first_name = student.first_name;
            last_name = student.last_name;
            register_at = student.register_at;
            supervisor_id = student.supervisor_id;
            mentor_id = student.mentor_id;
            mentor_name = student.mentor_name;
            advisor_id = student.advisor_id;
            advisor_name = student.advisor_name;
            join_course_date = student.join_course_date;
            exit_course_date = student.exit_course_date;
            graduate = student.graduate;
            start_date = student.start_date;
            end_date = student.end_date;
            plan_name = student.plan_name;
            tag = student.tag;

            // Assign tokens
            access_token = loginResponse.data.access_token;
            refresh_token = loginResponse.data.refresh_token;

            // Add working days
            if (loginResponse.data.days != null)
            {
                foreach (var day in loginResponse.data.days)
                {
                    working_days.Add(new WorkingDay
                    {
                        id = day.id,
                        course_plan_id = day.course_plan_id,
                        day_number = day.day_number,
                        start_time = day.start_time,
                        end_time = day.end_time,
                        day_name = day.day_name
                    });
                }
            }
        }
    }
}
