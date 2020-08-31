using System.Collections.Generic;

namespace AdfPipelineTrigger.Models
{
    public class Schedule
    {
        public List<int?> Hours { get; set; }
        public List<int?> Minutes { get; set; }
        public List<int?> MonthDays { get; set; }
        public List<int?> WeekDays { get; set; }
        public List<ScheduleRecurrence> ScheduleRecurrences { get; set; }
    }
}
