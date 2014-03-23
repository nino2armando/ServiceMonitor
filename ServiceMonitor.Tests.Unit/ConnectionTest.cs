using System;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    class ConnectionTest
    {
        private IConnection _connection;
        private TcpListener _listener;
        private TcpClient _client;
        private IRegister _register;
        private INotification _notification;

        [SetUp]
        public void SetUp()
        {
            _notification = MockRepository.GenerateMock<INotification>();
            _register = MockRepository.GenerateMock<IRegister>();
            _client = new TcpClient();
            _connection = new Connection(_client, _notification, _register);
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

        [Test, Ignore("Do not run")]
        public void TryPollServie_uses_TestConnection_to_validate_connection()
        {
            EstablishListener();
            _connection.TryPollServie(GetSubscriber());
            
            _connection.AssertWasCalled(a => a.TestConnection(11));       
        }

        [Test]
        public void TestConnection_Should_Return_Connection_Status()
        {
            EstablishListener();
            _client.Connect("127.0.0.1", 11111);
            var isConnected = _connection.TestConnection(1000000);

            Assert.IsTrue(isConnected);
            
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryPollServie_Should_Not_Accept_Frequency_Lower_than_a_second()
        {
            EstablishListener();
            _connection.TryPollServie(new Subscriber
                {
                    Service = GetCriteria(),
                    PollingFrequency = 1
                });          
        }

        [Test]
        public void IsServiceOutage_Can_detect_service_outage()
        {
            var isOutage = _connection.ServiceOutage(GetCriteria());
            Assert.IsTrue(isOutage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsServiceOutage_does_not_receive_null_argument()
        {
            var isOutage = _connection.ServiceOutage(null);
        }

        //[Test]
        //public void TryGetServiceStatus_should_return_correct_service_status()
        //{
        //    var criteria = GetCriteria();
        //    bool firstTry = _connection.TryGetServiceStatus(criteria);
        //    EstablishListener();
        //    bool secondTry = _connection.TryGetServiceStatus(criteria);

        //    Assert.IsFalse(firstTry);
        //    Assert.IsTrue(secondTry);
        //}

        [Test]
        public void IsIPV4_can_validate_v4_address()
        {
            string ip = "127.333.444.000";
            var isValid = _connection.IsIPV4(ip);

            Assert.IsFalse(isValid);
        }

        public Subscriber GetSubscriber()
        {
            return new Subscriber
                {
                    PollingFrequency = 10000000,
                    Service = GetCriteria()
                };
        }

        public Node GetCriteria()
        {
            return new Node
            {
                Ip = "127.0.0.1",
                Port = 11111,
                OutageStartTime = DateTimeOffset.Now,
                OutageEndTime = DateTimeOffset.Now.AddHours(1)
            };
        }

        public void EstablishListener()
        {
            _listener = new TcpListener(IPAddress.Loopback, 11111);
            _listener.Start();
        }
    }
}
