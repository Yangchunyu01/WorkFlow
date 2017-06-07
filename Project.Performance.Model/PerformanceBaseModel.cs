using System;
using System.Xml.Serialization;

namespace Project.Performance.Model
{
    [Serializable]
    public class PerformanceBaseModel
    {
        #region Properties
        [XmlIgnore]
        public Guid Id { get; set; }
        public string Operation { get; set; }
        public int Count { get; set; }
        public string NamePrefix { get; set; }
        [XmlIgnore]
        public bool IsQueryBehavior { get; set; }
        #endregion

        public PerformanceBaseModel()
        {
            Id = Guid.NewGuid();
            Count = 1;
        }
    }
}
