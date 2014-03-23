namespace ServiceMonitor.SharedContract.Contracts
{
    public class ServiceCriteria
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        // microSecond
        public int PollingFrequency { get; set; }
    }
}
