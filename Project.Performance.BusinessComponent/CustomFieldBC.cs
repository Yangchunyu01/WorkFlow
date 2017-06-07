using Project.Performance.Model;
using Project.Performance.Repository;

namespace Project.Performance.BusinessComponent
{
    public class CustomFieldBC : PerformanceBaseBC<CustomFieldModel>
    {
        public CustomFieldBC() : base(new CustomFieldCSOMClient())
        {
        }
    }
}
