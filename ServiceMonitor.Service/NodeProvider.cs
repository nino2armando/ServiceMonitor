using System;
using System.Collections.Generic;

namespace ServiceMonitor.Service
{
    public class NodeProvider : INodeProvider
    {
        private List<Node> AllServices { get; set; }

        public NodeProvider()
        {
            AllServices = new List<Node>();
        }
        /// <summary>
        /// Gets all available services.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> GetAllAvailableServices()
        {
            return AllServices;
        }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void AddService(Node service)
        {
            AllServices.Add(service);
        }

        /// <summary>
        /// Tests the vector.
        /// </summary>
        /// <returns></returns>
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
    }
}
