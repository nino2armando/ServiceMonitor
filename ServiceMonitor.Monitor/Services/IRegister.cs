using System.Collections.Generic;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Monitor.Services
{
    public interface IRegister
    {
        IEnumerable<Subscriber> GetAllSubsribers();
        IEnumerable<Subscriber> SameServiceSubscribers();
        void Enable(Subscriber caller);
    }
}