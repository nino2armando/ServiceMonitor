using System;
using System.Threading;

//namespace ServiceMonitor.Monitor.Helpers
//{
//    public class TimerManager
//    {
//        private static Timer _timer;
//        private static Action _action;

//        public static void Start(Action action, object data ,int freq)
//        {
//            _action = action;
//            _timer = new Timer(o => _action.BeginInvoke(data,), null, 0 , freq);
//        }


//        public static void Done()
//        {

//        }

//        public static void Stop()
//        {
//            _timer.Dispose();
//        }
//    }
//}
