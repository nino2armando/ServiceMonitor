using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceMonitor.Caller;
using ServiceMonitor.Notification.Services;
using ServiceMonitor.Service;

namespace ServiceMonitor.Tests.Unit
{
    [TestFixture]
    class NotificationTest
    {
        private INotification _notification;

        [SetUp]
        public void SetUp()
        {
            _notification = new Notification.Services.Notification();
        }

        [Test]
        public void AddToSubscriptionList_registers_a_new_Subscriber()
        {
            var sub = new Subscriber
            {
                Id = 1,
                Name = "Nino"
            };


            var allsubs = _notification.GetSubscriptionList();

            Assert.IsTrue(!allsubs.Any());
            _notification.AddToSubscriptionList(sub);
            Assert.IsTrue(allsubs.Count() == 1);
        }
    }
}
