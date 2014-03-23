using System;

namespace ServiceMonitor.Service
{
    public class Node
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public DateTimeOffset OutageStartTime { get; set; }
        public DateTimeOffset OutageEndTime { get; set; }
    }
}
