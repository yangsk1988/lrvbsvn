using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using VirtualBicycle.Vfs;

namespace VirtualBicycle.Core
{
    /// <summary>
    ///  定义一个资源的状态
    /// </summary>
    public enum ResourceState
    {
        /// <summary>
        ///  未加载
        /// </summary>
        Unloaded,
        /// <summary>
        ///  正在加载
        /// </summary>
        Loading,
        /// <summary>
        ///  已经加载
        /// </summary>
        Loaded,
        /// <summary>
        ///  正在卸载
        /// </summary>
        Unloading
    }

    /// <summary>
    ///  表示对一个资源对象的引用
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    public class ResourceHandle<T> : IDisposable
        where T : Resource
    {
        T resuorce;
        object syncHelper = new object();

        public ResourceHandle(T res)
        {
            res.Reference();
            this.resuorce = res;
        }

        ~ResourceHandle()
        {
            if (!Disposed)
            {
                Dispose();
            }
        }

        public T Resource
        {
            get
            {
                resuorce.Use();
                return resuorce;
            }
        }

        public static implicit operator T(ResourceHandle<T> res)
        {
            res.resuorce.Use();
            return res.resuorce;
        }

        public override string ToString()
        {
            return resuorce.ToString();
        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            lock (syncHelper)
            {
                if (!Disposed)
                {
                    resuorce.Dereference();
                    Disposed = true;
                }
                else
                {
                    throw new ObjectDisposedException(ToString());
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///  表示一个资源，如纹理、模型等
    /// </summary>
    public abstract class Resource : IDisposable
    {
        #region Resource Operations
        class ResourceLoader : ResourceOperation
        {
            public ResourceLoader(Resource resource)
                : base(resource)
            {
            }

            public override void Process()
            {
                if (Resource != null)
                {
                    Resource.load();
                    if (Resource.State == ResourceState.Loading)
                        Resource.State = ResourceState.Loaded;
                }
            }
        }
        class ResourceUnloader : ResourceOperation
        {
            public ResourceUnloader(Resource resource)
                : base(resource)
            {

            }

            public override void Process()
            {
                if (Resource != null)
                {
                    Resource.unload();
                    if (Resource.State == ResourceState.Unloaded)
                        Resource.State = ResourceState.Unloaded;
                }
            }
        }

        class ResoueceCacheReader : ResourceOperation
        {
            public ResoueceCacheReader(Resource resource)
                : base(resource)
            {

            }

            public override void Process()
            {
                if (Resource != null)
                {
                    Stream stream = Resource.cacheMem.ResourceLocation.GetStream;
                    Resource.ReadCacheData(new VirtualStream(stream));
                    if (Resource.State == ResourceState.Loading)
                        Resource.State = ResourceState.Loaded;
                }
            }
        }
        class ResoueceCacheWriter : ResourceOperation
        {
            public ResoueceCacheWriter(Resource resource)
                : base(resource)
            {

            }

            public override void Process()
            {
                if (Resource != null)
                {
                    Stream stream = Resource.cacheMem.ResourceLocation.GetStream;

                    Resource.WriteCacheData(new VirtualStream(stream));
                }
            }
        }
        #endregion

        /// <summary>
        ///  提供用于计算资源代数的功能
        /// </summary>
        class GenerationCalculator
        {
            List<TimeSpan> atLastMin;
            List<TimeSpan> atLast10Sec;
            List<TimeSpan> atLastSec;

            float frequency;

            int generation;

            GenerationTable table;

            public GenerationCalculator(GenerationTable table)
            {
                this.atLastMin = new List<TimeSpan>();
                this.atLast10Sec = new List<TimeSpan>();
                this.atLastSec = new List<TimeSpan>();
                this.table = table;

                this.generation = 3;
            }

            /// <summary>
            ///  获取资源的代数
            /// </summary>
            public int Generation
            {
                get { return generation; }
            }

            /// <summary>
            ///  通知资源被使用，更新代数
            /// </summary>
            public void Use(Resource resource)
            {
                #region 确定时间段内的使用次数

                TimeSpan time = EngineTimer.TimeSpan;

                atLastMin.Add(time);
                atLast10Sec.Add(time);
                atLastSec.Add(time);

                TimeSpan last10Sec = time - new TimeSpan(0, 0, 10);
                TimeSpan lastSec = time - new TimeSpan(0, 0, 1);
                TimeSpan lastMin = time - new TimeSpan(0, 1, 0);

                int startIdx = -1;
                for (int i = 0; i < atLastMin.Count; i++)
                {
                    if (atLastMin[i] < lastMin)
                        startIdx = i;
                }
                if (startIdx != -1)
                    atLastMin.RemoveRange(0, startIdx + 1);

                startIdx = -1;
                for (int i = 0; i < atLast10Sec.Count; i++)
                {
                    if (atLast10Sec[i] < last10Sec)
                        startIdx = i;
                }
                if (startIdx != -1)
                    atLast10Sec.RemoveRange(0, startIdx + 1);
               
                startIdx = -1;
                for (int i = 0; i < atLastSec.Count; i++)
                {
                    if (atLastSec[i] < lastSec)
                        startIdx = i;
                }
                if (startIdx != -1)
                    atLastSec.RemoveRange(0, startIdx + 1);
                #endregion

                #region 计算近期使用频率
                float rf = 0;
                if (atLastMin.Count == 1)
                {
                    rf = 1;
                }
                else if (atLastMin.Count > 0)
                {
                    int c = atLastMin.Count - 1;
                    rf = 1 / (float)((atLastMin[c] - atLastMin[c - 1]).Seconds);
                }

                frequency = (atLastSec.Count + atLast10Sec.Count / 10.0f + 0.5f * atLastMin.Count / 60.0f + rf) / 4.0f;

                #endregion

                int oldGeneration = generation;

                #region 由使用频率确定代数

                if (frequency > 0.001f)
                {
                    if (frequency > 0.01)
                    {
                        if (frequency > 0.1)
                        {
                            generation = 0;
                        }
                        else
                        {
                            generation = 1;
                        }
                    }
                    else
                    {
                        generation = 2;
                    }
                }
                else
                {
                    generation = 3;
                }
                #endregion

                if (oldGeneration != generation)
                {
                    if (oldGeneration != -1 && table[oldGeneration].Exists(resource))
                        table[oldGeneration].Remove(resource);

                    if (!table[generation].Exists(resource))
                        table[generation].Add(resource);
                }
            }
        }

        GenerationCalculator generation;

        ResourceManager manager;

        string hashString;

        int refCount;

        protected CacheMemory cacheMem;

        ResourceState resState;

        ResourceLoader resourceLoader;
        ResourceUnloader resourceUnloader;
        ResoueceCacheReader resourceCReader;
        ResoueceCacheWriter resourceCWriter;
        object syncHelper = new object();


        /// <summary>
        /// 获取资源的引用计数
        /// </summary>
        [Browsable(false)]
        public int ReferenceCount
        {
            get { return refCount; }
        }

        internal void Reference()
        {
            if (IsManaged)
                refCount++;
        }

        internal void Dereference()
        {
            if (IsManaged)
            {
                refCount--;

                if (refCount == 0)
                {
                    if (!Disposed)
                        Dispose();
                }
            }
        }


        /// <summary>
        /// 所有资源的名称都统一用该方法计算哈希代码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetHashCode(string name)
        {
            return name.ToUpperInvariant().GetHashCode();
        }


        public bool HasCache
        {
            get { return cacheMem.ResourceLocation != null; }
        }

        public virtual void SetCache(CacheMemory rl)
        {
            Stream stm = rl.ResourceLocation.GetStream;
            WriteCacheData(new VirtualStream(stm));

            cacheMem = rl;
        }

        public CacheMemory GetCache()
        {
            return cacheMem;
        }

        public virtual void ResetCache()
        {
            cacheMem = new CacheMemory();
        }

        protected abstract void ReadCacheData(Stream stream);
        protected abstract void WriteCacheData(Stream stream);

        /// <summary>
        ///  获取该资源的状态
        /// </summary>
        [Browsable(false)]
        public ResourceState State
        {
            get
            {
                lock (syncHelper)
                    return resState;
            }
            private set
            {
                lock (syncHelper)
                    resState = value;
            }
        }

        public override int GetHashCode()
        {
            if (hashString != null)
            {
                return GetHashCode(hashString);
            }
            return base.GetHashCode();
        }

        [Browsable(false)]
        public string HashString
        {
            get { return hashString; }
        }

        /// <summary>
        ///  创建一个不受管理的资源
        /// </summary>
        protected Resource()
        {
            resourceLoader = new ResourceLoader(this);
            resourceUnloader = new ResourceUnloader(this);
            resourceCReader = new ResoueceCacheReader(this);
            resourceCWriter = new ResoueceCacheWriter(this);
        }

        protected Resource(ResourceManager manager, string hashString)
            : this()
        {
            this.hashString = hashString;
            this.manager = manager;
            this.generation = new GenerationCalculator(manager.Table);
        }

        public int Generation
        {
            get
            {
                if (generation == null)
                    return -1;
                return generation.Generation;
            }
        }

        /// <summary>
        ///  获得管理该资源的资源管理器
        /// </summary>
        [Browsable(false)]
        public ResourceManager Manager
        {
            get { return manager; }
        }
        /// <summary>
        ///  获取该资源是否不受管理
        /// </summary>
        [Browsable(false)]
        public bool IsManaged
        {
            get { return manager != null; }
        }


        /// <summary>
        ///  获取一个System.Boolean，表示该资源是否已经加载
        /// </summary>
        [Browsable(false)]
        public bool IsLoaded
        {
            get { return resState == ResourceState.Loaded; }
        }


        /// <summary>
        ///  获取该资源所占内存的大小
        /// </summary>
        /// <returns></returns>
        public abstract int GetSize();

        /// <summary>
        ///  加载资源的实现
        /// </summary>
        protected abstract void load();

        /// <summary>
        ///  释放资源的实现
        /// </summary>
        protected abstract void unload();

        /// <summary>
        ///  获取一个System.Boolean，表示该资源是否可被释放
        /// </summary>
        [Browsable(false)]
        public virtual bool IsUnloadable
        {
            get { return true; }
        }

        /// <summary>
        ///  调用该方法表示将要使用这个资源。让资源做好准备（比如已释放的要加载好）
        /// </summary>
        public void Use()
        {
            if (IsManaged)
            {
                generation.Use(this);

                if (State != ResourceState.Loaded && State != ResourceState.Loading)
                {
                    Load();
                }
            }
        }

        /// <summary>
        ///  加载资源
        /// </summary>
        public void Load()
        {
            if (IsManaged)
            {
                generation.Use(this);

                State = ResourceState.Loading;

                if (HasCache)
                    manager.AddTask(resourceCReader);
                else
                    manager.AddTask(resourceLoader);

                manager.NotifyResourceLoaded(this);
            }
        }

        /// <summary>
        ///  释放资源
        /// </summary>
        public void Unload()
        {
            if (IsManaged)
            {
                State = ResourceState.Unloading;

                if (HasCache)
                    manager.AddTask(resourceCWriter);
                manager.AddTask(resourceUnloader);

                manager.NotifyResourceUnloaded(this);
            }
        }
        public void Reload()
        {
            if (IsManaged)
            {
                if (IsLoaded)
                {
                    Unload();
                    Load();
                }
            }
        }


        /// <summary>
        ///  销毁资源
        /// </summary>
        /// <param name="disposing">是否释放</param>
        protected virtual void dispose(bool disposing)
        {
            if (disposing && IsManaged)
            {
                Cache.Instance.Release(cacheMem);
                ResetCache();

                manager.NotifyResourceFinalizing(this);
            }
        }
        /// <summary>
        ///  获取一个System.Boolean，表示该资源是否已经销毁
        /// </summary>
        [Browsable(false)]
        public bool Disposed
        {
            get;
            private set;
        }


        #region IDisposable 成员

        /// <summary>
        ///  销毁资源
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~Resource()
        {
            if (!Disposed)
            {
                dispose(false);
                Disposed = true;
            }
        }

    }

}
