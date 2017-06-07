using System;

namespace Project.Performance.Model
{
    [Serializable]
    public class TaskModel : PerformanceBaseModel
    {
        public int Assignments { get; set; }
        public string TaskNamePrefix { get; set; }
        public Duration Duration { get; set; }
    }

    public class Duration
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
