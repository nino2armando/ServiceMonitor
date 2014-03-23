using System;
using ServiceMonitor.Caller;
using ServiceMonitor.SharedContract.Contracts;

namespace ServiceMonitor.Monitor.Services
{
    public interface IConnection : IDisposable
    {
        bool TryGetServiceStatus(ServiceCriteria criteria);
        void TryPollServie(Node caller);
        bool TestConnection(int pollingFrequency); // in microsecond
    }
}
