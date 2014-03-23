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
        public void SameServiceSubscribers_should_extract_simillar_subscribers_from_list()
        {
            Func<IEnumerable<Subscriber>> collection = GetTestSubscribers; 

            _register.Expect(a => a.GetAllSubsribers()).IgnoreArguments().Do(collection).Repeat.Once();

            
            
            var result = _register.SameServiceSubscribers();

            
        }

        public List<Subscriber> GetTestSubscribers()
        {
            return new List<Subscriber>
                {
                    new Subscriber
                        {
                            Criteria = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11111
                                }
                        },
                    new Subscriber
                        {
                            Criteria = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11111
                                }
                        },
                    new Subscriber
                        {
                            Criteria = new Node
                                {
                                    Ip = "127.0.0.1",
                                    Port = 11112
                                }
                        }
                };
        }

    }
}
