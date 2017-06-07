using Project.Performance.Model;

namespace Project.Performance.Repository
{
    public interface ICSOMClient<T> where T : PerformanceBaseModel
    {
        bool Create(T model);
        bool Delete(T model);
        int GetCount(XMLModel model);
        // IEnumerable<K> GetEntities();
    }
}
