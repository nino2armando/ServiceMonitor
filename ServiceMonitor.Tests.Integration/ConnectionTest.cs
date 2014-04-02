using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Integration
{
    [TestFixture]
    public class ConnectionTest
    {
        private IConnection _connection;
        private TcpListener _listener;
        private TcpClient _client;
        private INotification _notification;

        [SetUp]
        public void SetUp()
        {
            _notification = MockRepository.GenerateMock<INotification>();
            _listener = new TcpListener(IPAddress.Loopback, 11111);
            _client = new TcpClient();
            _connection = new Connection(_client, _notification);
        }

        [TearDown]
        public void TearDown()
        {
            if (_listener != null)
            {
                if (_listener.Server != null)
                {
                    _listener.Server.Close();
                    _listener.Stop();
                }
            }
        }

        [Test]
        public void TryConnect_Estabilishes_Connection_to_the_Listener()
        {
            EstablishListener();
            _connection.TryConnect(GetCaller().Service);
        }


        public Subscriber GetCaller()
        {
            return new Subscriber
                {
                    Name = "test caller",
                    Service = new Node
                        {
                            Ip = "127.0.0.1",
                            Port = 11111
                        }
                };
        }

        public void EstablishListener()
        {
            _listener.Start();
        }

    }
}
