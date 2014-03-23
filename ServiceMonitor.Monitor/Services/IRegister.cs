using System.Collections.Generic;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Monitor.Services
{
    public interface IRegister
    {
        IEnumerable<Node> GetAllSubsribers();
        void Enable(Node caller);
    }
}