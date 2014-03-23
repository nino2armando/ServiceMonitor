using ServiceMonitor.SharedContract.Contracts;

namespace ServiceMonitor.Caller
{
    public class Node
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ServiceCriteria Criteria { get; set; }
    }
}
