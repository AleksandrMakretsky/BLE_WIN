using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

using Windows.System.Threading;
//using SDKTemplate;

namespace WF_BLE_v5
{
     public class DisViewModel
     {

         public Windows.UI.Core.CoreDispatcher dispatcher;

        /*
         public DisViewModel(Windows.UI.Core.CoreDispatcher dispatcher)
         {
             this.dispatcher = dispatcher;

         }*/




}

        /*
    public class Dispatcher
    {
        private readonly BlockingCollection<Tuple<Delegate, object[]>> runQueue = new BlockingCollection<Tuple<Delegate, object[]>>();
        private readonly BlockingCollection<object> resultQueue = new BlockingCollection<object>();
        private readonly CancellationTokenSource source = new CancellationTokenSource();
        private readonly Task task;
        public Dispatcher()
        {
            Task.Run(() =>
            {
                using (source)
                using (runQueue)
                using (resultQueue)
                {
                    Debug.WriteLine("Dispatcher started with thread {0}", Thread.CurrentThread.ManagedThreadId);
                    while (!source.IsCancellationRequested)
                    {
                        var run = runQueue.Take(source.Token);
                        resultQueue.Add(run.Item1.DynamicInvoke(run.Item2));
                    }
                    Debug.WriteLine("Dispatcher ended");
                }
            });
        }

        public void Stop()
        {
            source.Cancel();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object Invoke(Delegate @delegate, params object[] @params)
        {
            runQueue.Add(new Tuple<Delegate, object[]>(@delegate, @params));
            return resultQueue.Take(source.Token);
        }
    }
    */

}