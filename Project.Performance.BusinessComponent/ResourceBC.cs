using Project.Performance.Model;
using Project.Performance.Repository;

namespace Project.Performance.BusinessComponent
{
    public class ResourceBC : PerformanceBaseBC<ResourcePoolModel>
    {
        public ResourceBC() : base(new ResourceCSOMClient())
        {
        }
    }
}
