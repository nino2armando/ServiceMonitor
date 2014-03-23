using System;
using System.Collections.Generic;
using System.Linq;
using ServiceMonitor.Caller;
using ServiceMonitor.Service;

namespace ServiceMonitor.Monitor.Services
{
    public class Register : IRegister
    {
        private List<Subscriber> AllSubscribers { get; set; }
        private IConnection _connection;

        public Register(IConnection connection)
        {
            if(connection == null)
                throw new ArgumentNullException("connection");

            _connection = connection;
            AllSubscribers = new List<Subscriber>();
        }

        public IEnumerable<Subscriber> GetAllSubsribers()
        {
            return AllSubscribers;
        }

        public IEnumerable<Subscriber> SameServiceSubscribers(IEnumerable<Subscriber> allRegistered)
        {
            var result = from x in allRegistered
                         group x by new {x.Service.Ip, x.Service.Port}
                         into grp
                         select new Subscriber
                             {
                                 Service = new Node
                                     {
                                         Ip = grp.Key.Ip,
                                         Port = grp.Key.Port
                                     }
                             };

            return result;
        }

        public void Enable(Subscriber caller)
        {
            if(caller == null)
                throw new ArgumentNullException("caller");

            AllSubscribers.Add(caller);
            _connection.CallSubscribedServies(AllSubscribers);
        }
    }
}
