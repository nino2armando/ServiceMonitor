using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using ServiceMonitor.Monitor.Models;
using ServiceMonitor.Monitor.Services;

namespace ServiceMonitor.Tests.Integration
{
    [TestFixture]
    public class ConnectionTest
    {
        private IConnection _connection;
        private TcpListener _listener;
        private TcpClient _client;

        [SetUp]
        public void SetUp()
        {
            _listener = new TcpListener(IPAddress.Loopback, 11111);
            _client = new TcpClient();
            _connection = new Connection(_client);
        }

        [Test]
        public void TestConnection_Should_Return_Connection_Status()
        {
            var client = new TcpClient();
            _connection = new Connection(client);

            var isConnected = _connection.TestConnection();

            Assert.IsTrue(isConnected);
        }

        [Test]
        public void Estabilish_Returns_Correct_Response()
        {
            //EstablishListener();
            var criteria = new ServiceCriteria
                {
                    Ip = "127.0.0.1",
                    Port = 11111,
                    PollingFrequency = 1
                };

            var response = _connection.Estabilish(criteria);
            

        }

        [Test]
        public void Kill_TcpClient()
        {
            _client.Client.Disconnect(true);
            _client.Close();
            
        }

        public void EstablishListener()
        {
            _listener.Start();
        }

        public void StopListener(TcpListener listener)
        {
            listener.Stop();
        }
    }
}
