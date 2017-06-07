using Project.Performance.BusinessComponent;
using Project.Performance.Model;

namespace Project.Performance.Service
{
    [ServiceAPI(typeof(CustomFieldModel))]
    public class CustomFieldService : IPerformanceBaseService<CustomFieldModel>
    {
        private CustomFieldBC bc = new CustomFieldBC();
        public bool Create(CustomFieldModel model)
        {
            return this.bc.Create(model);
        }

        public bool Delete(CustomFieldModel model)
        {
            return this.bc.Delete(model);
        }

        public int GetCount(XMLModel model)
        {
            return this.bc.GetCount(model);
        }
    }
}
