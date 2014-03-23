using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    class RegisterTest
    {
        private IConnection _connection;
        private IRegister _register;

        [SetUp]
        public void Setup()
        {
            _connection = MockRepository.GenerateStub<IConnection>();
            _register = new Register(_connection);
        }

        [Test]
        public void Enable_allows_for_Caller_subscription()
        {
            var subscriber = GetTestSubscribers().First();

            _register.Enable(subscriber);
            var allsubs = _register.GetAllSubsribers();

            _connection.AssertWasCalled(a => a.CallSubscribedServies(allsubs));
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Enable_throws_exception_for_null_argument()
        {
            _register.Enable(null);
        }

        [Test]
        public void SameServiceSubscribers_should_extract_simillar_subscribers_from_list()
        {
 
            var result = _register.SameServiceSubscribers(GetTestSubscribers());

            
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
                                    Port = 11111
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
