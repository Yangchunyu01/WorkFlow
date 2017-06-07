using System;

namespace Project.Performance.Model
{
    [Serializable]
    public class LookupTableModel : PerformanceBaseModel
    {
        public int NumberOfValues { get; set; }
        public int OutlineLevels { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
