using System;
using System.Collections.Generic;
using System.Linq;
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
            _connection.ServicePollPassed(new Subscriber
                {
                    Service = GetCriteria(),
                    PollingFrequency = 1
                });
        }

        [Test]
        public void IsServiceOutage_Should_only_check_if_outage_is_set()
        {
            var node = new Node();
            var isOutage = _connection.ServiceOutage(node);
            Assert.IsFalse(isOutage);
        }

        [Test]
        public void IsServiceOutage_Can_detect_service_outage()
        {
            var node = new Node
                {
                    Ip = "127.0.0.1",
                    Port = 11111,
                    OutageStartTime = DateTimeOffset.UtcNow.AddHours(-1),
                    OutageEndTime = DateTimeOffset.UtcNow.AddHours(1)
                };
            var isOutage = _connection.ServiceOutage(node);
            Assert.IsTrue(isOutage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsServiceOutage_does_not_receive_null_argument()
        {
            var isOutage = _connection.ServiceOutage(null);
        }

        [Test]
        public void CallSubscribedServies_sends_notification_on_Failure()
        {
            var thisSubsriber = new Subscriber
                        {
                            PollingFrequency = 100000,
                            Service = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11111,
                                    OutageStartTime = DateTime.Now,
                                    OutageEndTime = DateTime.Now.AddHours(-1)
                                }
                        
                };

            
            _notification.Expect(a => a.GetSubscriptionList()).Return(GetSubscribers());
            _connection.CallSubscriberService(thisSubsriber);
            _notification.AssertWasCalled(
                a =>
                a.Send(thisSubsriber.Service.Ip, thisSubsriber.Service.Port));
        }

        [Test]
        public void CallSubscribedServies_should_wait_for_gracetime_before_sending_notification()
        {
            //EstablishListener();
            _connection.CallSubscriberService(GetSubscribers().First());

        }

        [Test]
        public void CallSubscribedServies_Does_Not_send_notification_Service_Outage_Is_Specified()
        {
            var subscriber = GetSubscribers().ToList()[1];

            _connection.CallSubscriberService(subscriber);

            _notification.AssertWasNotCalled(a => a.Send(subscriber.Service.Ip, subscriber.Service.Port));
        }

        //[Test]
        //public void CallSubscribedServies_Calls_ServicePollPassed_if_serviceOutage_is_specified()
        //{
        //    var thisSubsriber = new Subscriber
        //    {
        //        PollingFrequency = 100000,
        //        Service = new Node
        //        {
        //            Ip = "127.0.0.1",
        //            Port = 11111,
        //            OutageStartTime = DateTime.Now.AddHours(-1),
        //            OutageEndTime = DateTime.Now.AddHours(1)
        //        }

        //    };

        //    var connection = MockRepository.GenerateMock<IConnection>();

        //    connection.Expect(a => a.ServicePollPassed(thisSubsriber)).IgnoreArguments().Return(false);
        //    connection.CallSubscriberService(thisSubsriber);
        //    connection.AssertWasCalled(a => a.ServicePollPassed(thisSubsriber), o => o.Repeat.Once());
        //    connection.VerifyAllExpectations();
        //}

        //[Test]
        //public void CallSubscribedServies_Calls_ServicePollPassed_twice_if_graceTime_specified()
        //{
        //    var thisSubsriber = new Subscriber
        //    {
        //        GraceTime = 1000000,
        //        PollingFrequency = 100000,
        //        Service = new Node
        //        {
        //            Ip = "127.0.0.1",
        //            Port = 11111,
        //            OutageStartTime = DateTime.Now.AddHours(-1),
        //            OutageEndTime = DateTime.Now.AddHours(1)
        //        }

        //    };

        //    var connection = MockRepository.GenerateMock<IConnection>();

        //    connection.Expect(a => a.ServiceOutage(thisSubsriber.Service)).IgnoreArguments().Return(false);
        //    connection.CallSubscriberService(thisSubsriber);
        //    connection.AssertWasCalled(a => a.ServicePollPassed(thisSubsriber), options => options.Repeat.Times(1));
        //}

        //[Test]
        //public void CallSubscribedServies_makes_a_call_to_ServicePollPassed()
        //{
        //    var thisSubsriber = new Subscriber
        //    {
        //        PollingFrequency = 100000,
        //        Service = new Node
        //        {
        //            Ip = "127.0.0.1",
        //            Port = 11111,
        //            OutageStartTime = DateTime.Now,
        //            OutageEndTime = DateTime.Now.AddHours(-1)
        //        }

        //    };
        //    var connection = MockRepository.GenerateMock<IConnection>();
        //    connection.Expect(a => a.CallSubscriberService(thisSubsriber));
        //    connection.AssertWasCalled(a => a.ServicePollPassed(thisSubsriber));
        //}

        [Test]
        public void TryGetServiceStatus_should_return_correct_service_status()
        {
            var passed = _connection.ServicePollPassed(GetSubscribers().First());

            Assert.IsFalse(passed);
        }

        [Test]
        public void IsIPV4_can_validate_v4_address()
        {
            string ip = "127.333.444.000";
            var isValid = _connection.IsIPV4(ip);

            Assert.IsFalse(isValid);
        }

        public IEnumerable<Subscriber> GetSubscribers()
        {
            return new List<Subscriber>
                {
                    new Subscriber
                        {
                            GraceTime = 1000,
                            PollingFrequency = 10000000,
                            Service = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11111
                                }
                        },

                    new Subscriber
                        {
                            PollingFrequency = 20000000,
                            Service = GetCriteria()
                        },
                };
        }

        public Node GetCriteria()
        {
            return new Node
            {
                Ip = "127.0.0.1",
                Port = 11111,
                OutageStartTime = DateTimeOffset.UtcNow,
                OutageEndTime = DateTimeOffset.UtcNow.AddHours(1)
            };
        }

        public void EstablishListener()
        {
            _listener = new TcpListener(IPAddress.Loopback, 11111);
            _listener.Start();
        }
    }
}
