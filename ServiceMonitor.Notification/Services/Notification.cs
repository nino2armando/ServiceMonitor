using System;
using System.Collections.Generic;
using System.Linq;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Notification.Services
{
    public class Notification : INotification
    {
        private List<Subscriber> SubscriptionList { get; set; }

        public Notification()
        {
            SubscriptionList = new List<Subscriber>();
        }

        public void AddToSubscriptionList(Subscriber caller)
        {
            SubscriptionList.Add(caller);
        }


        public IEnumerable<Subscriber> GetSubscriptionList()
        {
            return SubscriptionList;
        }

        public void Send(string ip, int port)
        {
            // all these subscribers would get notified
            var allsubs = SubscriptionList.Where(a => a.Service.Ip == ip && a.Service.Port == port );

            // now send notification to these subscribers
            
            // todo: not too sure how the messaging is going to look like

            throw new System.NotImplementedException();
        }
    }
}
