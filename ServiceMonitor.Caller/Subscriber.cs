using ServiceMonitor.Service;

namespace ServiceMonitor.Caller
{
    public class Subscriber
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int PollingFrequency { get; set; } // millisecond
        public int GraceTime { get; set; } // millisecond
        public Node Service { get; set; }
    }
}
