using System;

namespace Project.Performance.Model
{
    [Serializable]
    public class ProjectModel : PerformanceBaseModel
    {
        public string TemplateName { get; set; }
        public TaskModel TaskModel { get; set; }
        public int ProjectTeamCount { get; set; }
    }
}
