using System.Collections.Generic;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Notification.Services
{
    public interface INotification
    {
        void Send(IEnumerable<Node> callers);
    }
}
