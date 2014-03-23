using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Monitor.Services
{
    public class Connection : IConnection
    {
        private TcpClient _client { get; set; }
        private readonly INotification _notification;
        private readonly IRegister _register;

        private bool _disposed;
        private const int MIN_FREQUENCY = 1000;

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

        public void TryConnect(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if(!IsIPV4(node.Ip))
                throw new InvalidDataException("Ip");

            var address = IPAddress.Parse(node.Ip);

             _client.Connect(address, node.Port);
        }

        public bool ServicePollPassed(Subscriber subscriber)
        {
            if (subscriber.Service == null)
                throw new ArgumentNullException("criteria");

            if(!IsIPV4(subscriber.Service.Ip))
                throw new InvalidDataException("Ip");
            
            if (subscriber.PollingFrequency < MIN_FREQUENCY)    
                throw new ArgumentOutOfRangeException("frequency most be higher than a second");

            while (true)
            {
                try
                {
                    TryConnect(subscriber.Service);
                }
                catch (Exception)
                {
                    return false;
                }

                var reader = new StreamReader(_client.GetStream());

                while (TestConnection(subscriber.PollingFrequency))
                {
                    try
                    {
                        var timer = new Timer(o => reader.ReadLine() , null, 0, subscriber.PollingFrequency);
                    }
                    catch
                    {                    
                        // check for the gracetime
                        // after time we should perhaps do one last check
                        return false;
                    }               
                }
            }
        }

        public void CallSubscribedServies(IEnumerable<Subscriber> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                var passed = ServicePollPassed(subscriber);
                if (!passed)
                {
                    if (!ServiceOutage(subscriber.Service))
                        _notification.Send(subscriber);
                }
            }
        }

        public bool ServiceOutage(Node service)
        {
            if(service == null)
                throw new ArgumentNullException("service");

            var now = DateTimeOffset.UtcNow;

            if ((now > service.OutageStartTime) && (now < service.OutageEndTime))
            {
                return true;
            }
            return false;
        }

        public bool TestConnection(int frequency)
        {
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

        public bool IsIPV4(string value)
        {
            IPAddress address;
            if (IPAddress.TryParse(value, out address))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return true;
                }
            }
            return false;
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