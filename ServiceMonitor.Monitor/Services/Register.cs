using System;
using System.Collections.Generic;
using System.Linq;
using ServiceMonitor.Caller;

namespace ServiceMonitor.Monitor.Services
{
    public class Register : IRegister
    {
        private IConnection _connection;

        public Register(IConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
        }

        public IEnumerable<Subscriber> GetAllSubsribers()
        {
            // call some service to get all subscribers
            return new List<Subscriber>();
        }

        public IEnumerable<Subscriber> SameServiceSubscribers()
        {
            var allRegistered = GetAllSubsribers();
            var sameServiceSubscribers = new List<Subscriber>();

            foreach (var subscriber in allRegistered)
            {
                sameServiceSubscribers.AddRange(
                    allRegistered.Where(
                        sub =>
                        subscriber.Criteria.Ip == sub.Criteria.Ip &&
                        subscriber.Criteria.Port == sub.Criteria.Port));
            }

            return sameServiceSubscribers;
        }

        public void Enable(Subscriber caller)
        {
            _connection.TryPollServie(caller);
        }
    }
}
