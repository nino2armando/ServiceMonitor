using System;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Models;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.SharedContract.Contracts;

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

        [Test]
        public void Estabilish_Returns_Correct_Response()
        {
            //EstablishListener();
            var caller = GetCaller();
            _connection.TryPollServie(caller);
            
        }


        public Node GetCaller()
        {
            return new Node
                {
                    Name = "test caller",
                    Criteria = new ServiceCriteria
                        {
                            Ip = "127.0.0.1",
                            Port = 11111,
                            PollingFrequency = 1
                        }
                };
        }

        public void EstablishListener()
        {
            _listener.Start();
        }

    }
}
