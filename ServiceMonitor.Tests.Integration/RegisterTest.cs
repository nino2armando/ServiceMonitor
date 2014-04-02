using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Integration
{
    [TestFixture]
    public class RegisterTest
    {
        private IRegister _register;
        private IConnection _connection;
        private INodeProvider _nodeProvider;
        private INotification _notification;

        [SetUp]
        public void SetUp()
        {
            _connection = MockRepository.GenerateMock<IConnection>();
            _nodeProvider = MockRepository.GenerateMock<INodeProvider>();
            _notification = MockRepository.GenerateMock<INotification>();
            _register = new Register(_connection, _nodeProvider, _notification);
        }

        [Test]
        public void Enable_Makes_call_back_to_the_Specified_method()
        {
            
            _nodeProvider.Expect(a => a.GetAllAvailableServices()).Return(TestVector());
            var sub = GetTestSubscribers().First();

            _register.Enable(sub);

        }

        public IEnumerable<Node> TestVector()
        {
            return new List<Node>()
                {
                    new Node
                        {
                            Ip = "127.0.0.1",
                            Port = 11111,
                        },
                    new Node
                        {
                            OutageStartTime = new DateTimeOffset(2014, 3, 17, 1, 32, 0, new TimeSpan(-5, 0, 0)),
                            OutageEndTime = new DateTimeOffset(2014, 4, 17, 1, 32, 0, new TimeSpan(-5, 0, 0)),
                            Ip = "127.0.0.1",
                            Port = 11111,
                        }
                };
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
