using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;


namespace ServiceMonitor.Monitor.Services
{
    public class Connection : IConnection
    {
        private TcpClient _client { get; set; }
        private INotification _notification;
        private IRegister _register;

        private bool _disposed;
        private const int MIN_FREQUENCY = 1000000;

        public Connection(TcpClient client, INotification notification, IRegister register)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            if(register == null)
                throw new ArgumentNullException("register");

            _disposed = false;
            _client = client ?? new TcpClient();
            _notification = notification;
            _register = register;
        }

        public bool TryGetServiceStatus(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var address = IPAddress.Parse(node.Ip);
            bool isup = true;

            try
            {
                _client.Connect(address, node.Port);
            }
            catch (Exception)
            {
                isup = false;
            }

            return isup;
        }

        public void TryPollServie(Subscriber caller)
        {
            if (caller.Criteria == null)
                throw new ArgumentNullException("criteria");

            var address = IPAddress.Parse(caller.Criteria.Ip);

            while (true)
            {
                string line = string.Empty;

                _client.Connect(address, caller.Criteria.Port);
                var reader = new StreamReader(_client.GetStream());

                while (TestConnection(caller.Criteria.PollingFrequency))
                {
                    try
                    {
                        line = reader.ReadLine();
                        // we should send a service online here perhaps
                    }
                    catch
                    {
                        // we should get all the subscribers and send notification to the ones that are subscribing to the same service

                        var simillarSubscribers = _register.SameServiceSubscribers();
                        _notification.Send(simillarSubscribers);
                    }
                    
                }
            }
            throw new NotImplementedException();
        }

        public bool TestConnection(int frequency)
        {
            if (frequency < MIN_FREQUENCY)
            {
                throw new ArgumentOutOfRangeException("frequency most be higher than a second");
            }

            try
            {
                if (_client != null && _client.Client != null && _client.Client.Connected)
                {
                    // if client is disconnected
                    if (_client.Client.Poll(frequency, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // client is disconnected
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        _client.Close();
                        _client = null;
                    }
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}