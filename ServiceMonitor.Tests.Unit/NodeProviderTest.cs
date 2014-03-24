using System.Linq;
using NUnit.Framework;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    public class NodeProviderTest
    {
        private INodeProvider _nodeProvider;

        [SetUp]
        public void SetUp()
        {
            _nodeProvider = new NodeProvider();
        }

        [Test]
        public void AddService_registers_a_new_Service()
        {
            var node = new Node
                {
                    Ip = "127.1.1.1"
                };


            var allservices = _nodeProvider.GetAllAvailableServices();
            
            Assert.IsTrue(!allservices.Any());
            _nodeProvider.AddService(node);
            Assert.IsTrue(allservices.Count() == 1);
        }
    }
}
