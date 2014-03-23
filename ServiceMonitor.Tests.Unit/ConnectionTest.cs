using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceMonitor.Monitor.Models;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.SharedContract.Contracts;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    class ConnectionTest
    {
        private IConnection _connection;
        private TcpListener _listener;
        private TcpClient _client;

        [SetUp]
        public void SetUp()
        {    
            _client = new TcpClient();
            _connection = new Connection(_client);
        }

        // add tear down for stop connection

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
        public void TestConnection_Should_Not_Accept_Frequency_Lower_than_a_second()
        {
            EstablishListener();
            _client.Connect("127.0.0.1", 11111);
            var isConnected = _connection.TestConnection(1);          
        }

        [Test]
        public void TryGetServiceStatus_should_return_correct_service_status()
        {
            var criteria = GetCriteria();
            bool firstTry = _connection.TryGetServiceStatus(criteria);
            EstablishListener();
            bool secondTry = _connection.TryGetServiceStatus(criteria);

            Assert.IsFalse(firstTry);
            Assert.IsTrue(secondTry);
        }

        public ServiceCriteria GetCriteria()
        {
            return new ServiceCriteria
            {
                Ip = "127.0.0.1",
                Port = 11111,
                PollingFrequency = 1
            };
        }

        public void EstablishListener()
        {
            _listener = new TcpListener(IPAddress.Loopback, 11111);
            _listener.Start();
        }
    }
}
