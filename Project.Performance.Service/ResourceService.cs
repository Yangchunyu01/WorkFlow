using Project.Performance.BusinessComponent;
using Project.Performance.Model;

namespace Project.Performance.Service
{
    [ServiceAPI(typeof(ResourcePoolModel))]
    public class ResourceService : IPerformanceBaseService<ResourcePoolModel>
    {
        private ResourceBC bc = new ResourceBC();
        public bool Create(ResourcePoolModel model)
        {
            return this.bc.Create(model);
        }

        public bool Delete(ResourcePoolModel model)
        {
            return this.bc.Delete(model);
        }

        public int GetCount(XMLModel model)
        {
            return this.bc.GetCount(model);
        }
    }
}
