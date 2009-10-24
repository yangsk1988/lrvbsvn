using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Collections;
using System.Threading;

namespace VirtualBicycle.Core
{
    /// <summary>
    ///  资源分代管理，代数越小，资源使用越频繁
    /// </summary>
    class GenerationTable : IDisposable
    {
        class GenerationTableCommander : IDisposable
        {
            Thread thread;

            struct Task
            {
                public TimeSpan actTime;
                public Resource res;
            }

            float[] checkInterval;
            Queue<Task>[] queues;

            public GenerationTableCommander(GenerationTable table)
            {
                thread = new Thread(Main);

                // 最后一代不用管，因为最后一代的对象不可能再进化了
                queues = new Queue<Task>[GenerationTable.MaxGeneration - 1];
                queues[0] = new Queue<Task>();
                queues[1] = new Queue<Task>();
                queues[2] = new Queue<Task>();
            }

            private void Main(object state)
            {
                while (!Disposed)
                {
                    
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

        const int MaxGeneration = 4;

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


        ExistTable<Resource>[] gen;
        GenerationTableCommander commander;

        public GenerationTable()
        {
            gen = new ExistTable<Resource>[MaxGeneration];

            for (int i = 0; i < MaxGeneration; i++)
            {
                gen[i] = new ExistTable<Resource>();
            }

            commander = new GenerationTableCommander(this);
        }

        public ExistTable<Resource> this[int index]
        {
            get
            {
                return gen[index];
            }
        }

        public void ApplyChecking(int generation, Resource res)
        {

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
