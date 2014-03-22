using System;
using System.Net.Sockets;
using ServiceMonitor.Monitor.Models;

namespace ServiceMonitor.Monitor.Services
{
    public interface IConnection : IDisposable
    {
        Response Estabilish(ServiceCriteria criteria);
        bool TestConnection();
    }
}
