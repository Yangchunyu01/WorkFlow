using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Project.Performance.Model;
using Project.Performance.Utility;


namespace Project.Performance.Service
{
    public class ConfigService
    {
        public ProjectService ProjectService { get; private set; }
        public ResourceService ResourceService { get; private set; }
        public CustomFieldService CustomFieldService { get; private set; }
        public LookupTableService LookupTableService { get; private set; }

        private static ConfigService instance;

        private ConfigService()
        {
            this.ProjectService = new ProjectService();
            this.ResourceService = new ResourceService();
            this.CustomFieldService = new CustomFieldService();
            this.LookupTableService = new LookupTableService();
        }

        public static ConfigService Instance
        {
            get
            {
                if (ConfigService.instance == null)
                {
                    ConfigService.instance = new ConfigService();
                }
                return ConfigService.instance;
            }
        }

        public void ExecuteConfigTasks(XMLModel xmlModel)
        {
            // Properties in XMLModel: ProjectModelList, ResourcePoolModelList etc.
            foreach (PropertyInfo property in xmlModel.GetType().GetProperties())
            {
                // PerformanceBaseModel : ProjectModel, ResourcePoolModel etc.
                foreach (PerformanceBaseModel baseModel in (property.GetValue(xmlModel) as IEnumerable<PerformanceBaseModel>))
                {
                    var serviceObj =
                        this.GetType().GetProperties().First(x => x.PropertyType.GetCustomAttributes(typeof(ServiceAPIAttribute), false).Any() &&
                            x.PropertyType.GetCustomAttribute<ServiceAPIAttribute>().ModelType == baseModel.GetType())
                        .GetGetMethod().Invoke(this, null);

                    MethodInfo method = serviceObj.GetType().GetMethod(baseModel.Operation);
                    if (method != null)
                    {
                        method.Invoke(serviceObj, new object[1] { baseModel });
                    }
                    else
                    {
                        LoggerHelper.Instance.Fail(string.Format("Cannot find behavior {0} to execute!", baseModel.Operation));
                    }
                }
            }
        }
    }
}
