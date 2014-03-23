using System;
using ServiceMonitor.Caller;
using ServiceMonitor.Caller;
using ServiceMonitor.Service;

namespace ServiceMonitor.Monitor.Services
{
    public interface IConnection : IDisposable
    {
        void TryConnect(Node node);
        void TryPollServie(Subscriber subscriber);
        bool TestConnection(int pollingFrequency); // in microsecond
        bool IsIPV4(string value);
        bool ServiceOutage(Node service);
    }
}
