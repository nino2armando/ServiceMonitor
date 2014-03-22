namespace ServiceMonitor.Monitor.Models
{
    public class ServiceCriteria
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        // microSecond
        public int PollingFrequency { get; set; }
    }
}
