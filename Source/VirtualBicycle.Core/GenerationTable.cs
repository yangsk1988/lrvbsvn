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

        enum ManageState 
        {
            Off,
            RequiresSynchronize,
            Ready,
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

        /// <summary>
        ///  代数计算线程与管理线程同步
        /// </summary>
        object syncHelper3 = new object();

        ExistTable<Resource>[] gen;
        List<Resource> genList;

        Thread guThread;
        Thread mgrThread;
        ResourceManager manager;
        ManageState manageState;

        [MTAThread()]
        private void GenerationUpdate_Main(object state)
        {
            const int passTimeLimit = 4000;

            TimeSpan timeStart = EngineTimer.TimeSpan;

            while (!Disposed)
            {
                bool isSyncing;

                lock (syncHelper3) 
                {
                    isSyncing = manageState == ManageState.RequiresSynchronize;
                }


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

                        if (res.generation.GenerationOutOfTime(ref timeStart))
                        {
                            int og = res.generation.generation;

                            res.generation.UpdateGeneration();
                            int ng = res.generation.generation;

                            if (ng != og)
                            {
                                UpdateGeneration(og, ng, res);
                                //Console.WriteLine("{0}由{1}改变至{2}", res.HashString, og.ToString(), ng.ToString());
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

                if (isSyncing) 
                {
                    lock (syncHelper3) 
                    {
                        manageState = ManageState.Ready;
                    }
                }

                Thread.Sleep(100);
            }
        }

        [MTAThread()]
        private void Manage_Main(object state)
        {
            while (!Disposed)
            {
                bool flag;

                lock (syncHelper3)
                {
                    flag = manageState == ManageState.Ready;

                    if (flag)
                    {
                        manageState = ManageState.Off;
                    }
                }

                if (flag)
                {
                    int predictCSize = manager.UsedCacheSize;

                    while (predictCSize > manager.TotalCacheSize)
                    {
                        for (int i = 3; i >= 1 && predictCSize > manager.TotalCacheSize; i--)
                        {
                            foreach (Resource r in gen[i])
                            {
                                if (r != null && r.State == ResourceState.Loaded && r.IsUnloadable)
                                {
                                    predictCSize -= r.GetSize();
                                    r.Unload();

                                    if (predictCSize <= manager.TotalCacheSize)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (manager.UsedCacheSize > manager.TotalCacheSize)
                    {
                        ManageSwitch = true;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public bool ManageSwitch 
        {
            get
            {
                lock (syncHelper3)
                    return manageState != ManageState.Off;
            }
            set
            {
                lock (syncHelper3)
                {
                    if (manageState == ManageState.Off)
                    {
                        manageState = ManageState.RequiresSynchronize;
                    }
                }
            }
        }

        public GenerationTable(ResourceManager mgr)
        {
            gen = new ExistTable<Resource>[MaxGeneration];
            genList = new List<Resource>();
            for (int i = 0; i < MaxGeneration; i++)
            {
                gen[i] = new ExistTable<Resource>();
            }
            manager = mgr;

            guThread = new Thread(GenerationUpdate_Main);
            guThread.Name = "Generation Update Thread for" + manager.ToString();
            guThread.SetApartmentState(ApartmentState.MTA);
            guThread.Start();

            mgrThread = new Thread(Manage_Main);
            mgrThread.Name = "Resource Management Thread for" + manager.ToString();
            mgrThread.SetApartmentState(ApartmentState.MTA);
            mgrThread.Start();
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
