using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ServiceMonitor.Monitor.Models;

namespace ServiceMonitor.Monitor.Services
{
    public class Connection : IConnection
    {
        private TcpClient _client { get; set; }
        private bool _disposed;

        public Connection(TcpClient client)
        {
            if(client == null)
                throw new ArgumentNullException("client");

            _disposed = false;
            _client = client;
        }

        public Response Estabilish(ServiceCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            var address = IPAddress.Parse(criteria.Ip);

/*            while (true)
            {
                string line = string.Empty;

                _client.Connect(address, criteria.Port);
                var reader = new StreamReader(_client.GetStream());

                while (TestConnection())
                {
                    line = reader.ReadLine();
                }

            }*/

            string line = string.Empty;
            _client.Connect(address, criteria.Port);

            var reader = new StreamReader(_client.GetStream());
            bool test;
            if (TestConnection())
            {
                test = true;
            }
            else
            {
                test = false;
            }

            return new Response();
        }

        public bool TestConnection()
        {
            bool isConnected = true;

            if (_client.Client.Poll(0, SelectMode.SelectRead))
            {
                if (!_client.Connected) { isConnected = false; }
                else
                {
                    byte[] b = new byte[1];

                    try
                    {
                        if (_client.Client.Receive(b, SocketFlags.Peek) == 0)
                        {
                            // client disconected
                            isConnected = false;
                        }
                    }
                    catch (Exception)
                    {
                        isConnected = false;
                        // throw;
                    }
                }
            }
            return isConnected;
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