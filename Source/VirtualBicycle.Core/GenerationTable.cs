using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VirtualBicycle.Collections;

namespace VirtualBicycle.Core
{
    /// <summary>
    ///  资源分代管理，代数越小，资源使用越频繁
    /// </summary>
    class GenerationTable : IDisposable
    {
        class GenerationTableMaintance : IDisposable
        {
            struct Task
            {
                public TimeSpan actTime;
                public Resource res;
            }

            /// <summary>
            ///  各个代的请求队列
            /// </summary>
            Queue<Task>[] queues;
            ExistTable<Resource>[] etables;
            Thread thread;
            GenerationTable table;

            /// <summary>
            ///  对queues的线程锁
            /// </summary>
            object syncHelper = new object();

            public GenerationTableMaintance(GenerationTable table)
            {
                this.table = table;
                thread = new Thread(Main);

                // 最后一代不用管，因为最后一代的对象不可能再进化了
                queues = new Queue<Task>[GenerationTable.MaxGeneration - 1];
                etables = new ExistTable<Resource>[GenerationTable.MaxGeneration - 1];

                for (int i = 0; i < queues.Length; i++)
                {
                    queues[i] = new Queue<Task>();
                    etables[i] = new ExistTable<Resource>();
                }

                thread.Name = "Generation Maintance";
                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();
            }

            private void Main(object state)
            {
                TimeSpan time = EngineTimer.TimeSpan;
                
                // 记录各个代的记录起始时间
                TimeSpan[] timeStart = new TimeSpan[GenerationTable.MaxGeneration - 1];
                for (int i = 0; i < timeStart.Length; i++)
                {
                    timeStart[i] = time;
                }

                // 对各个代的时间计数器
                float[] timeCount = new float[GenerationTable.MaxGeneration - 1];

                while (!Disposed)
                {
                    time = EngineTimer.TimeSpan;

                    for (int i = 0; i < timeCount.Length; i++)
                    {
                        timeCount[i] = (float)(time - timeStart[i]).TotalSeconds;
                    }

                    for (int i = 0; i < GenerationTable.MaxGeneration - 1; i++) 
                    {
                        if (timeCount[i] > GetGenerationLifeTime(i))
                        {
                            lock (syncHelper)
                            {
                                while (queues[i].Count > 0)
                                {
                                    Task t = queues[i].Dequeue();
                                    Resource res = t.res;

                                    int g = res.Generation;

                                    if (g != i)
                                    {
                                        etables[i].Remove(res);
                                        table.UpdateGeneration(i, g, res);
                                    }
                                    else
                                    {
                                        queues[i].Enqueue(t);
                                    }
                                }
                            }
                            timeStart[i] = time;
                        }
                    }

                    Thread.Sleep(10);
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
                if (!Disposed)
                {
                    Disposed = true;
                }
            }

            #endregion

            public void ApplyChecking(int generation, Resource res)
            {
                if (!etables[generation].Exists(res))
                {
                    Task t;
                    t.actTime = EngineTimer.TimeSpan + TimeSpan.FromSeconds(GetGenerationLifeTime(generation));
                    t.res = res;

                    lock (syncHelper)
                    {
                        queues[generation].Enqueue(t);
                    }
                    etables[generation].Add(res);
                }
            }

            public bool MaintanceCompleted 
            {
                get
                {
                    bool result = true;

                    lock (syncHelper)
                    {
                        for (int i = 0; i < queues.Length; i++)
                        {
                            if (queues[i].Count > 0) 
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    return result;
                }
            }
        }

        class RefEqualityComparer : IEqualityComparer<Resource>
        {
            #region IEqualityComparer<Resource> 成员

            public bool Equals(Resource x, Resource y)
            {
                return object.ReferenceEquals(x, y);
            }

            public int GetHashCode(Resource obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        public const int MaxGeneration = 4;

        static int[] GenerationLifeTime = new int[MaxGeneration] { 10, 60, 300, int.MaxValue };

        /// <summary>
        ///  获取特定代的生存周期
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        static internal int GetGenerationLifeTime(int i)
        {
            return GenerationLifeTime[i];
        }

        /// <summary>
        ///  对gen的线程锁
        /// </summary>
        object syncHelper = new object();

        ExistTable<Resource>[] gen;
        GenerationTableMaintance commander;

        public GenerationTable()
        {
            gen = new ExistTable<Resource>[MaxGeneration];

            for (int i = 0; i < MaxGeneration; i++)
            {
                gen[i] = new ExistTable<Resource>();
            }

            commander = new GenerationTableMaintance(this);
        }

        public ExistTable<Resource> this[int index]
        {
            get
            {
                return gen[index];
            }
        }

        public void AddResource(Resource res) 
        {
            int g = res.Generation;
            if (g != -1)
            {
                lock (syncHelper)
                {
                    gen[g].Add(res);
                }
            }
        }
        public void RemoveResource(Resource res)
        {
            int g = res.Generation;

            if (g != -1)
            {
                lock (syncHelper)
                {
                    gen[g].Remove(res);
                }
            }
        }
        public void ApplyChecking(int generation, Resource res)
        {
            commander.ApplyChecking(generation, res);
        }
        public void UpdateGeneration(int oldGeneration, int newGeneration, Resource resource)
        {
            lock (syncHelper)
            {
                if (oldGeneration != -1 && gen[oldGeneration].Exists(resource))
                    gen[oldGeneration].Remove(resource);

                if (!gen[newGeneration].Exists(resource))
                    gen[newGeneration].Add(resource);
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
            if (!Disposed) 
            {
                commander.Dispose();
                Disposed = true;
            }
        }

        #endregion
    }

}
