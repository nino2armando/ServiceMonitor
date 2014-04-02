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

        /// <summary>
        /// Adds to subscription list.
        /// </summary>
        /// <param name="caller">The caller.</param>
        public void AddToSubscriptionList(Subscriber caller)
        {
            SubscriptionList.Add(caller);
        }


        /// <summary>
        /// Gets the subscription list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Subscriber> GetSubscriptionList()
        {
            return SubscriptionList;
        }

        /// <summary>
        /// Sends the specified ip.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Send(string ip, int port)
        {
            // all these subscribers would get notified

            var allsubs = SubscriptionList.Where(a => a.Service.Ip == ip && a.Service.Port == port);

            // now send notification to these subscribers
            
            // todo: not too sure how the messaging is going to look like

            //throw new System.NotImplementedException();
        }
    }
}
