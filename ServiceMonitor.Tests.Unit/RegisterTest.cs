using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    class RegisterTest
    {
        private IConnection _connection;
        private IRegister _register;
        private INodeProvider _nodeProvider;
        private INotification _notification;

        [SetUp]
        public void Setup()
        {
            _connection = MockRepository.GenerateStub<IConnection>();
            _nodeProvider = MockRepository.GenerateMock<INodeProvider>();
            _notification = MockRepository.GenerateMock<INotification>();
            _register = new Register(_connection, _nodeProvider, _notification);
        }

        [Test]
        public void Enable_requests_all_servcies_from_NodeProvider()
        {
            _nodeProvider.Expect(a => a.GetAllAvailableServices()).Return(new List<Node>
                {
                    new Node
                        {
                            Ip = "127.0.0.1",
                            Port = 11111,
                        }
                });
            var subscriber = GetTestSubscribers().First();

            _register.Enable(subscriber);

            _nodeProvider.AssertWasCalled(a => a.GetAllAvailableServices());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Enable_throws_exception_for_null_argument()
        {
            _register.Enable(null);
        }

        [Test]
        public void Enable_only_calls_CallSubscriberService_for_newServices()
        {
            var nodeList = new List<Node>
                {
                    new Node
                        {
                            Ip = "127.0.0.1",
                            Port = 11112,
                        }
                };

            var sub = new Subscriber
                {
                    Service = nodeList.First()
                };

            _nodeProvider.Expect(a => a.GetAllAvailableServices()).Return(nodeList);
            _register.Enable(sub);

            // passing same service
            _connection.AssertWasNotCalled(a => a.CallSubscriberService(sub));
        }

        [Test]
        public void Enable_Register_caller_for_notifications_for_new_and_old_notifications()
        {
            var nodeList = new List<Node>
                {
                    new Node
                        {
                            Ip = "127.0.0.1",
                            Port = 11112,
                        }
                };
            var sub = GetTestSubscribers().First();
            _nodeProvider.Expect(a => a.GetAllAvailableServices()).Return(nodeList);
            _register.Enable(sub);
            _notification.AssertWasCalled(a => a.AddToSubscriptionList(sub));
        }

        [Test]
        public void Enable_Will_calls_all_expected_services_for_newServices()
        {
            var nodeList = new List<Node>
                {
                    new Node
                        {
                            Ip = "192.168.0.1",
                            Port = 11112,
                        }
                };

            var sub = new Subscriber
            {
                Service = GetTestSubscribers().First().Service
            };

            _nodeProvider.Expect(a => a.GetAllAvailableServices()).Return(nodeList);
            _register.Enable(sub);

            // passing same service
            _connection.AssertWasCalled(a => a.CallSubscriberService(sub));
            _nodeProvider.AssertWasCalled(a => a.AddService(sub.Service));
        }

        public List<Subscriber> GetTestSubscribers()
        {
            return new List<Subscriber>
                {
                    new Subscriber
                        {
                            Name = "s1",
                            Service = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11112
                                }
                        },
                    new Subscriber
                        {
                            Name = "s2",
                            Service = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11111
                                }
                        },
                    new Subscriber
                        {
                            Name = "s3",
                            Service = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11112
                                }
                        }
                };
        }

    }
}
