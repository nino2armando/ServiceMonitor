using System;
using System.Collections.Generic;
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

        public IEnumerable<Node> GetAllSubsribers()
        {
            // call some service to get all subscribers
            throw new NotImplementedException();
        }

        public void Enable(Node caller)
        {
            _connection.TryPollServie(caller);
        }
    }
}
