using System;
using ServiceMonitor.Caller;
using ServiceMonitor.Caller;
using ServiceMonitor.Service;

namespace ServiceMonitor.Monitor.Services
{
    public interface IConnection : IDisposable
    {
        bool TryGetServiceStatus(Node node);
        void TryPollServie(Subscriber caller);
        bool TestConnection(int pollingFrequency); // in microsecond
    }
}
