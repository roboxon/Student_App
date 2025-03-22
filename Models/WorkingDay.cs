using System.Collections.Generic;

namespace Student_App.Models
{
    public class WorkingDay
    {
        public int id { get; set; }
        public int course_plan_id { get; set; }
        //day number 1 = Monday, 2 = Tuesday, 3 = Wednesday, 4 = Thursday, 5 = Friday, 6 = Saturday, 7 = Sunday
        public int day_number { get; set; }
        public string? start_time { get; set; }
        public string? end_time { get; set; }
        public string? day_name { get; set; }
    }
}
