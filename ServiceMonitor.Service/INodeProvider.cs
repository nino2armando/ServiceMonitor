using System.Collections.Generic;

namespace ServiceMonitor.Service
{
    public interface INodeProvider
    {
        IEnumerable<Node> GetAllAvailableServices();
        void AddService(Node service);
    }
}