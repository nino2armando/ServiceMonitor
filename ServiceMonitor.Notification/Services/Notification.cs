using System.Collections.Generic;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Model;

namespace ServiceMonitor.Notification.Services
{
    public class Notification : INotification
    {
        public void Send(IEnumerable<Node> callers)
        {
            throw new System.NotImplementedException();
        }
    }
}
