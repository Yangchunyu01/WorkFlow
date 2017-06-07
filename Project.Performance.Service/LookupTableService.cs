using Project.Performance.BusinessComponent;
using Project.Performance.Model;

namespace Project.Performance.Service
{
    [ServiceAPI(typeof(LookupTableModel))]
    public class LookupTableService : IPerformanceBaseService<LookupTableModel>
    {
        private LookupTableBC bc = new LookupTableBC();
        public bool Create(LookupTableModel model)
        {
            return this.bc.Create(model);
        }

        public bool Delete(LookupTableModel model)
        {
            return this.bc.Delete(model);
        }

        public int GetCount(XMLModel model)
        {
            return this.bc.GetCount(model);
        }
    }
}
