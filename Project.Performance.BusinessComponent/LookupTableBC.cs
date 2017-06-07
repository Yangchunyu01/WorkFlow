using Project.Performance.Model;
using Project.Performance.Repository;

namespace Project.Performance.BusinessComponent
{
    public class LookupTableBC : PerformanceBaseBC<LookupTableModel>
    {
        public LookupTableBC() : base(new LookupTableCSOMClient())
        {
        }
    }
}
