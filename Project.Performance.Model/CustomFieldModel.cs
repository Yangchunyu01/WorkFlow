using System;

namespace Project.Performance.Model
{
    [Serializable]
    public class CustomFieldModel : PerformanceBaseModel
    {
        #region Properties
        public string Name { get; set; }
        public string EntityType { get; set; }
        public string ValueType { get; set; }
        public string InputType { get; set; }
        public string LookupTable { get; set; }
        public string Formula { get; set; }
        #endregion

        public CustomFieldModel() : base()
        {
        }
    }
}
