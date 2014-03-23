using ServiceMonitor.Service;

namespace ServiceMonitor.Caller
{
    public class Subscriber
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Node Criteria { get; set; }
    }
}
