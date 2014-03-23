using System.Collections.Generic;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Model;

namespace ServiceMonitor.Notification.Services
{
    public class Notification : INotification
    {
        public void Send(Subscriber caller)
        {
            throw new System.NotImplementedException();
        }

        public void Send(IEnumerable<Subscriber> callers)
        {
            throw new System.NotImplementedException();
        }
    }
}
