using System;
using System.Collections.Generic;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Notification.Services
{
    public interface INotification
    {
        void AddToSubscriptionList(Subscriber caller);
        IEnumerable<Subscriber> GetSubscriptionList();
        void Send(string ip, int port);
    }
}
