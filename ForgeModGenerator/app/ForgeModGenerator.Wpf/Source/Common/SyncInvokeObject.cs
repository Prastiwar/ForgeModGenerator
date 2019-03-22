using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ForgeModGenerator
{
    internal class SyncInvokeObject : ISynchronizeInvoke
    {
        private class DispatcherAsyncResultAdapter : IAsyncResult
        {
            public DispatcherAsyncResultAdapter(DispatcherOperation operation) => Operation = operation;

            public DispatcherAsyncResultAdapter(DispatcherOperation operation, object state) : this(operation) => AsyncState = state;

            public DispatcherOperation Operation { get; }
            public object AsyncState { get; }

            public WaitHandle AsyncWaitHandle => null;
            public bool CompletedSynchronously => false;
            public bool IsCompleted => Operation.Status == DispatcherOperationStatus.Completed;
        }

        public SyncInvokeObject(Dispatcher dispatcher) => this.dispatcher = dispatcher;

        static SyncInvokeObject() => Default = new SyncInvokeObject(Application.Current.Dispatcher);

        public static SyncInvokeObject Default { get; }

        private Dispatcher dispatcher;

        public bool InvokeRequired => dispatcher.Thread != Thread.CurrentThread;

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            if (args != null && args.Length > 1)
            {
                object[] argsSansFirst = GetArgsAfterFirst(args);
                DispatcherOperation op = dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0], argsSansFirst);
                return new DispatcherAsyncResultAdapter(op);
            }
            else
            {
                return args != null
                    ? new DispatcherAsyncResultAdapter(dispatcher.BeginInvoke(DispatcherPriority.Normal, method, args[0]))
                    : new DispatcherAsyncResultAdapter(dispatcher.BeginInvoke(DispatcherPriority.Normal, method));
            }
        }

        public object EndInvoke(IAsyncResult result)
        {
            if (!(result is DispatcherAsyncResultAdapter res))
            {
                throw new InvalidCastException();
            }

            while (res.Operation.Status != DispatcherOperationStatus.Completed || res.Operation.Status == DispatcherOperationStatus.Aborted)
            {
                Thread.Sleep(50);
            }
            return res.Operation.Result;
        }

        public object Invoke(Delegate method, object[] args)
        {
            if (args != null && args.Length > 1)
            {
                return dispatcher.Invoke(DispatcherPriority.Normal, method, args[0], GetArgsAfterFirst(args));
            }
            else
            {
                return args != null
                    ? dispatcher.Invoke(DispatcherPriority.Normal, method, args[0])
                    : dispatcher.Invoke(DispatcherPriority.Normal, method);
            }
        }

        private static object[] GetArgsAfterFirst(object[] args)
        {
            object[] result = new object[args.Length - 1];
            Array.Copy(args, 1, result, 0, args.Length - 1);
            return result;
        }
    }
}
