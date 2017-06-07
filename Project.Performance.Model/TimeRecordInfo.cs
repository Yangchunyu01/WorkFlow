using System;

namespace Project.Performance.Model
{
    public class TimeRecordInfo
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get { return EndTime - StartTime; } }
    }
}
