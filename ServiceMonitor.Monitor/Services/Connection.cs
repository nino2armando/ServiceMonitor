using System;
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

        private bool _disposed;
        private const int MIN_FREQUENCY = 1000;

        public Connection(TcpClient client, INotification notification)
        {
            if (notification == null)
                throw new ArgumentNullException("notification");

            if(notification == null)
                throw new ArgumentNullException("notification");

            _disposed = false;
            _client = client ?? new TcpClient();
            _notification = notification;
        }

        /// <summary>
        /// Tries the connect.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="System.ArgumentNullException">node</exception>
        /// <exception cref="System.IO.InvalidDataException">Ip</exception>
        public void TryConnect(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if(!IsIPV4(node.Ip))
                throw new InvalidDataException("Ip");

            var address = IPAddress.Parse(node.Ip);

             _client.Connect(address, node.Port);
        }

        /// <summary>
        /// Services the poll passed.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">criteria</exception>
        /// <exception cref="System.IO.InvalidDataException">Ip</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">frequency most be higher than a second</exception>
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
                string line = string.Empty;

                while (TestConnection(subscriber.PollingFrequency))
                {
                    try
                    {
                        // this is how we check to see if conection is still online
                        line = reader.ReadLine();
                    }
                    catch
                    {                    
                        return false;
                    }               
                }
            }
        }

        /// <summary>
        /// Calls the subscriber service.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public bool CallSubscriberService(Subscriber subscriber)
        {
            bool passed;   
    
            passed = ServicePollPassed(subscriber);
            if (!passed)
            {
                if (!ServiceOutage(subscriber.Service))
                {
                    // check for the  grace time ??
                    // if exist wait before notification send
                    // check to see if service online before end of gracetime
                    // if grace time < polling frequency extra check is required

                    if (subscriber.GraceTime > 0)
                    {
                        Thread.Sleep(subscriber.GraceTime);
                        passed = ServicePollPassed(subscriber);
                        if (subscriber.GraceTime < subscriber.PollingFrequency)
                        {
                            passed = ServicePollPassed(subscriber);
                        }
                        if (!passed)
                        {
                            _notification.Send(subscriber.Service.Ip, subscriber.Service.Port);
                        }
                    }
                    else
                    {
                        _notification.Send(subscriber.Service.Ip, subscriber.Service.Port);
                    }
                }
            }
            return passed;
        }

        /// <summary>
        /// Services the outage.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">service</exception>
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

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether [is ip v4] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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