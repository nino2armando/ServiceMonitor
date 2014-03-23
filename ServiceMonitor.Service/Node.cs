namespace ServiceMonitor.Service
{
    public class Node
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        // microSecond
        public int PollingFrequency { get; set; }
    }
}
