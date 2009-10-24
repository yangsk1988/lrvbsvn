using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VirtualBicycle.Core
{
    public abstract class ResourceOperation
    {
        Resource resource;

        public Resource Resource
        {
            get { return resource; }
        }

        protected ResourceOperation(Resource res) 
        {
            this.resource = res;
            
        }

        public abstract void Process();
    }

    /// <summary>
    ///  资源异步处理器
    /// </summary>
    class AsyncProcessor : IDisposable
    {
        Queue<ResourceOperation> opQueue;

        Thread processThread;

        object syncHelper = new object();


        public AsyncProcessor() 
        {
            opQueue = new Queue<ResourceOperation>();

            processThread = new Thread(Main);
            processThread.SetApartmentState(ApartmentState.MTA);
            processThread.Start();
        }

        [MTAThread()]
        private void Main(object state) 
        {
            while (!Disposed)
            {
                ResourceOperation resOp = null;

                lock (syncHelper)
                {
                    if (opQueue.Count > 0)
                    {
                        resOp = opQueue.Dequeue();
                    }
                    else 
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                }

                if (resOp != null)
                {
                    resOp.Process();
                }
            }
        }

        public void AddTask(ResourceOperation op) 
        {
            opQueue.Enqueue(op);
        }

        public bool TaskCompleted 
        {
            get
            {
                lock (syncHelper)
                {
                    return opQueue.Count == 0;
                }
            }
        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Disposed = true;
        }

        #endregion
    }
}
