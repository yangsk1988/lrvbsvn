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
            processThread.Name = "AsyncProcessor";

#if !XBOX
            processThread.SetApartmentState(ApartmentState.MTA);
#endif

            processThread.Start();
        }

        [MTAThread()]
        private void Main() 
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
                }

                if (resOp != null)
                {
                    resOp.Process();
                }
                else 
                {
                    Thread.Sleep(10); 
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
