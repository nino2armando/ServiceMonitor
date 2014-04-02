using System;
using System.Net.Sockets;
using ServiceMonitor.Caller;
using ServiceMonitor.Monitor.Services;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpClient = new TcpClient();
            INodeProvider _nodeProvider = new NodeProvider();
            INotification _notification = new Notification.Services.Notification();

            IConnection _connection = new Connection(tcpClient,_notification);

            IRegister _register = new Register(_connection, _nodeProvider, _notification);

            var subscriber = new Subscriber()
                {
                    GraceTime = 1000,
                    Id = 11,
                    Name = "Console.Call",
                    PollingFrequency = 1000,
                    Service = new Node()
                        {
                            Ip = "127.0.0.1",
                            Port = 1111
                        }
                };
            
            _register.Enable(subscriber);
            Console.ReadKey();
        }
    }
}
