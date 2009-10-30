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
        //class GenerationTableMaintance : IDisposable
        //{
        //    struct Task
        //    {
        //        public TimeSpan actTime;
        //        public Resource res;
        //    }

        //    /// <summary>
        //    ///  各个代的请求队列
        //    /// </summary>
        //    Queue<Task>[] queues;
        //    ExistTable<Resource>[] etables;
        //    Thread thread;
        //    GenerationTable table;

        //    /// <summary>
        //    ///  对queues的线程锁
        //    /// </summary>
        //    object syncHelper = new object();

        //    public GenerationTableMaintance(GenerationTable table)
        //    {
        //        this.table = table;
        //        thread = new Thread(Main);

        //        // 最后一代不用管，因为最后一代的对象不可能再进化了
        //        queues = new Queue<Task>[GenerationTable.MaxGeneration - 1];
        //        etables = new ExistTable<Resource>[GenerationTable.MaxGeneration - 1];

        //        for (int i = 0; i < queues.Length; i++)
        //        {
        //            queues[i] = new Queue<Task>();
        //            etables[i] = new ExistTable<Resource>();
        //        }

        //        thread.Name = "Generation Maintance";
        //        thread.SetApartmentState(ApartmentState.MTA);
        //        thread.Start();
        //    }

        //    private void Main(object state)
        //    {
        //        TimeSpan time = EngineTimer.TimeSpan;
                
        //        // 记录各个代的记录起始时间
        //        TimeSpan[] timeStart = new TimeSpan[GenerationTable.MaxGeneration - 1];
        //        for (int i = 0; i < timeStart.Length; i++)
        //        {
        //            timeStart[i] = time;
        //        }

        //        // 对各个代的时间计数器
        //        float[] timeCount = new float[GenerationTable.MaxGeneration - 1];

        //        while (!Disposed)
        //        {
        //            time = EngineTimer.TimeSpan;

        //            for (int i = 0; i < timeCount.Length; i++)
        //            {
        //                timeCount[i] = (float)(time - timeStart[i]).TotalSeconds;
        //            }

        //            for (int i = 0; i < GenerationTable.MaxGeneration - 1; i++) 
        //            {
        //                if (timeCount[i] > GetGenerationLifeTime(i))
        //                {
        //                    lock (syncHelper)
        //                    {
        //                        while (queues[i].Count > 0)
        //                        {
        //                            Task t = queues[i].Dequeue();
        //                            Resource res = t.res;

        //                            int g = res.Generation;

        //                            if (g != i)
        //                            {
        //                                etables[i].Remove(res);
        //                                table.UpdateGeneration(i, g, res);
        //                            }
        //                            else
        //                            {
        //                                queues[i].Enqueue(t);
        //                            }
        //                        }
        //                    }
        //                    timeStart[i] = time;
        //                }
        //            }

        //            Thread.Sleep(10);
        //        }
        //    }

        //    #region IDisposable 成员

        //    public bool Disposed
        //    {
        //        get;
        //        private set;
        //    }

        //    public void Dispose()
        //    {
        //        if (!Disposed)
        //        {
        //            Disposed = true;
        //        }
        //    }

        //    #endregion

        //    public void ApplyChecking(int generation, Resource res)
        //    {
        //        if (!etables[generation].Exists(res))
        //        {
        //            Task t;
        //            t.actTime = EngineTimer.TimeSpan + TimeSpan.FromSeconds(GetGenerationLifeTime(generation));
        //            t.res = res;

        //            lock (syncHelper)
        //            {
        //                queues[generation].Enqueue(t);
        //            }
        //            etables[generation].Add(res);
        //        }
        //    }

        //    public bool MaintanceCompleted 
        //    {
        //        get
        //        {
        //            bool result = true;

        //            lock (syncHelper)
        //            {
        //                for (int i = 0; i < queues.Length; i++)
        //                {
        //                    if (queues[i].Count > 0) 
        //                    {
        //                        result = false;
        //                        break;
        //                    }
        //                }
        //            }
        //            return result;
        //        }
        //    }
        //}

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

        internal static readonly float[] GenerationLifeTime = new float[MaxGeneration] { 10, 60, 300, 800 };

        /// <summary>
        ///  对gen的线程锁
        /// </summary>
        object syncHelper = new object();
        /// <summary>
        ///  对genList的线程锁
        /// </summary>
        object syncHelper2 = new object();

        ExistTable<Resource>[] gen;
        List<Resource> genList;

        Thread thread;

        [MTAThread()]
        private void MMain(object state)
        {
            const int passTimeLimit = 4000;

            TimeSpan timeStart = EngineTimer.TimeSpan;

            while (!Disposed)
            {
                TimeSpan time = EngineTimer.TimeSpan;

                int count;
                lock (syncHelper2)
                {
                    count = genList.Count;
                }

                if (count > 0)
                {
                    int loopCount = 0;

                    int remainingTime = passTimeLimit;
                    int perObjTime = passTimeLimit / count;
                    int actlObjTime = Math.Max(1, Math.Min(perObjTime, 10));

                    for (int j = 0; j < count; j++)
                    {
                        Resource res;

                        lock (syncHelper2)
                        {
                            res = genList[j];
                        }

                        if (res.generation.GenerationOutOfTime(ref timeStart)) // bug
                        {
                            int og = res.generation.generation;

                            res.generation.UpdateGeneration();
                            int ng = res.generation.generation;

                            if (ng != og)
                            {
                                UpdateGeneration(og, ng, res);
                                Console.WriteLine("{0}由{1}改变至{2}", res.HashString, og.ToString(), ng.ToString());
                                //EngineConsole.Instance.Write("资源{0}由{1}改变至{2}", res.HashString, og.ToString(), ng.ToString());
                            }
                        }

                        if (loopCount++ % 10 == 0)
                        {
                            timeStart = EngineTimer.TimeSpan;
                            remainingTime -= (int)(timeStart - time).TotalMilliseconds;

                            loopCount = 0;
                        }

                        if (perObjTime >= 1 && remainingTime > 0)
                        {
                            Thread.Sleep(actlObjTime);
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }
        

        public GenerationTable()
        {
            gen = new ExistTable<Resource>[MaxGeneration];
            genList = new List<Resource>();
            for (int i = 0; i < MaxGeneration; i++)
            {
                gen[i] = new ExistTable<Resource>();
            }
            thread = new Thread(MMain);
            thread.Name = "Generation Maintance";
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
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
                lock (syncHelper2) 
                {
                    genList.Add(res);
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
                lock (syncHelper2)
                {
                    genList.Remove(res);
                }
            }
        }
        //public void ApplyChecking(int generation, Resource res)
        //{
        //    commander.ApplyChecking(generation, res);
        //}
        internal void UpdateGeneration(int oldGeneration, int newGeneration, Resource resource)
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
                //commander.Dispose();
                Disposed = true;
            }
        }

        #endregion
    }

}
