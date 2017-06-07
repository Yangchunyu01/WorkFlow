using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Project.Performance.Model
{
    [Serializable]
    [XmlRoot("Models")]
    public class XMLModel
    {
        [XmlArray("ProjectTemplates"), XmlArrayItem("ProjectTemplate")]
        public List<ProjectModel> ProjectModelList { get; set; }

        [XmlArray("ResourcePools"), XmlArrayItem("ResourcePool")]
        public List<ResourcePoolModel> ResourcePoolModelList { get; set; }

        [XmlArray("CustomFieldModels"), XmlArrayItem("CustomFieldModel")]
        public List<CustomFieldModel> CustomFieldModelList { get; set; }

        [XmlArray("LookUpTables"), XmlArrayItem("LookUpTable")]
        public List<LookupTableModel> LookupTableModelList { get; set; }
    }
}
