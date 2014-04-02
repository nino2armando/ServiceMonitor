using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Monitor.Services
{
    public class Register : IRegister
    {
        private IConnection _connection;
        private INodeProvider _nodeProvider;
        private INotification _notification;
        private Func<Subscriber, bool> method;
        private Timer _timer;

        public Register(IConnection connection, INodeProvider nodeProvider, INotification notification)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            if (nodeProvider == null)
                throw new ArgumentNullException("nodeProvider");

            if(notification == null)
                throw new ArgumentNullException("notification");

            _nodeProvider = nodeProvider;
            _connection = connection;
            _notification = notification;
        }

        /// <summary>
        /// Enables the specified caller.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <exception cref="System.ArgumentNullException">caller</exception>
        public void Enable(Subscriber caller)
        {
            if(caller == null)
                throw new ArgumentNullException("caller");

            var allServices = _nodeProvider.GetAllAvailableServices();

            method = _connection.CallSubscriberService;

            // we should check if service is not already available
            // otherwize just add the caller to notification list
            var serviceExist = allServices.Any(a => a.Ip == caller.Service.Ip && a.Port == caller.Service.Port);

            // either case the caller gets registered for notifications
            _notification.AddToSubscriptionList(caller);

            if (!serviceExist)
            {
                // if service does not exist, add to the service list
                _nodeProvider.AddService(caller.Service);
                _timer = new Timer(o => method.BeginInvoke(caller, CallBack, method), null, 0, caller.PollingFrequency);
            }
        }

        public void CallBack(IAsyncResult asyncResult)
        {
            var target = (Func<Subscriber, bool>) asyncResult.AsyncState;
            bool result = target.EndInvoke(asyncResult);
            if (result == false)
            {
                Stop();
            }
        }

        public void Stop()
        {
            _timer.Dispose();
        }
    }
}
