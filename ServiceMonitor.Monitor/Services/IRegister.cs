using System.Collections.Generic;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Monitor.Services
{
    public interface IRegister
    {
        void Enable(Subscriber caller);
    }
}