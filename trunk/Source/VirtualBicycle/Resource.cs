using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using VirtualBicycle.IO;

namespace VirtualBicycle
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
    ///  表示一个资源，如纹理、模型等
    /// </summary>
    /// <remarks>
    /// 资源分两种，引用和实体
    /// 引用引用着资源实体
    /// 引用不受管理，但使用时会影响实体的状态
    /// 资源管理器根据实体资源的状态对其进行管理
    /// </remarks>
    public abstract class Resource : IDisposable
    {
        ResourceState resState;
        TimeSpan creationTime;
        TimeSpan lastAccessed;
        int accessTimes;

        float useFrequency;

        ResourceManager manager;

        string hashString;
        //int hashCode;

        bool isUnmanaged;

        Resource resourceEntity;

        int entityRefCount;

        protected CacheMemory cacheMem;

        /// <summary>
        /// 如果是资源实体，获取资源实体的引用计数
        /// </summary>
        [Browsable(false)]
        public int ReferenceCount
        {
            get { return entityRefCount; }
            set { entityRefCount = value; }
        }
        /// <summary>
        ///  如果是引用，获取资源实体
        /// </summary>
        [Browsable(false)]
        public Resource ResourceEntity
        {
            get { return resourceEntity; }
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

        public abstract void ReadCacheData(Stream stream);
        public abstract void WriteCacheData(Stream stream);

        /// <summary>
        ///  获取该资源的状态
        /// </summary>
        [Browsable(false)]
        public ResourceState State
        {
            get { return resState; }
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
        ///  不受管理
        /// </summary>
        protected Resource()
        {
            IsUnmamaged = true;
        }

        /// <summary>
        ///  获取一个System.Boolean，表示该资源是否允许动态加载/卸载
        /// </summary>
        [Browsable(false)]
        public bool AllowDynamicLoading
        {
            get;
            protected set;
        }

        /// <summary>
        ///  创建引用，不受管理
        /// </summary>
        protected Resource(ResourceManager manager, Resource resourceEntity)
        {
            this.hashString = string.Empty;
            this.resourceEntity = resourceEntity;

            this.IsUnmamaged = true;

            this.resourceEntity.ReferenceCount++;
        }

        protected Resource(ResourceManager manager, string hashString)
        {
            this.AllowDynamicLoading = true;
            this.hashString = hashString;
            this.manager = manager;
        }


        protected Resource(ResourceManager manager, string hashString, bool allowdl)
        {
            this.AllowDynamicLoading = allowdl;
            this.hashString = hashString;
            this.manager = manager;
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
        public bool IsUnmamaged
        {
            get { return isUnmanaged; }
            private set { isUnmanaged = value; }
        }

        /// <summary>
        ///  获取一个System.Boolean，表示该资源是否是资源实体(ResourceEntity)
        /// </summary>
        [Browsable(false)]
        public bool IsResourceEntity
        {
            get { return resourceEntity == null; }
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
        ///  获取该资源的使用频率
        /// </summary>
        [Browsable(false)]
        public float UseFrequency
        {
            get { return useFrequency; }
        }

        /// <summary>
        ///  获取该资源所占内存的大小
        /// </summary>
        /// <returns></returns>
        public abstract int GetSize();

        /// <summary>
        ///  获取该资源的创建时间
        /// </summary>
        [Browsable(false)]
        public TimeSpan CreationTime
        {
            get { return creationTime; }
        }

        /// <summary>
        ///  获取上次访问该资源的时间
        /// </summary>
        [Browsable(false)]
        public TimeSpan LastAccessedTime
        {
            get { return lastAccessed; }
        }

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
            if (!IsResourceEntity)
            {
                resourceEntity.Use();
            }
            else
            {
                if (!isUnmanaged)
                {
                    lastAccessed = EngineTimer.TimeSpan;
                    accessTimes++;

                    float seconds = (float)((lastAccessed - creationTime).TotalSeconds);
                    useFrequency = seconds < 1 ? float.MaxValue : ((float)accessTimes) / seconds;

                    if (resState == ResourceState.Unloaded)
                    {
                        //manager.Manage();
                        Load();
                    }
                }
            }
        }

        /// <summary>
        ///  加载资源
        /// </summary>
        public void Load()
        {
            if (IsResourceEntity)
            {
                if (!isUnmanaged)
                {
                    creationTime = EngineTimer.TimeSpan;
                    lastAccessed = creationTime;

                    manager.Manage();

                    if (HasCache)
                    {
                        Stream stream = cacheMem.ResourceLocation.GetStream;
                        ReadCacheData(new VirtualStream(stream));
                    }
                    else
                    {
                        load();
                    }

                    manager.NotifyResourceLoaded(this);
                    resState = ResourceState.Loaded;
                }
            }
        }

        /// <summary>
        ///  释放资源
        /// </summary>
        public void Unload()
        {
            if (IsResourceEntity)
            {
                if (!isUnmanaged)
                {
                    if (HasCache)
                    {
                        Stream stream = cacheMem.ResourceLocation.GetStream;
                        WriteCacheData(new VirtualStream(stream));
                        unload();
#warning review
                    }
                    else
                    {
                        unload();
                    }
                    //manager.NotifyResourceUnloaded(this);
                    resState = ResourceState.Unloaded;
                }
            }
        }
        public void Reload()
        {
            if (IsResourceEntity)
            {
                if (!isUnmanaged)
                {
                    if (IsLoaded)
                    {
                        Unload();
                        Load();
                    }
                }
            }
        }


        /// <summary>
        ///  销毁资源
        /// </summary>
        /// <param name="disposing">是否释放</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && manager != null)
            {
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
                Dispose(true);
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
                Dispose(false);
                Disposed = true;
            }
        }

    }
    public abstract class ResourceManager
    {
        /// <summary>
        ///  受管理的资源哈希表
        /// </summary>
        Dictionary<string, Resource> hashTable;

        /// <summary>
        ///  受管理的资源的列表
        /// </summary>
        List<Resource> objects;

        /// <summary>
        ///  最大允许的缓存大小
        /// </summary>
        int totalCacheSize;

        /// <summary>
        ///  已经使用的缓存大小
        /// </summary>
        int curUsedCache;


        /// <summary>
        ///  记录管理资源的频率
        /// </summary>
        int manageFrequency;

        /// <summary>
        ///  记录管理资源的次数
        /// </summary>
        int manageTimes;


        /// <summary>
        ///  是否允许资源的动态加载
        /// </summary>
        bool allowDynLd = true;

        /// <summary>
        ///  以默认缓存大小创建一个资源管理器
        /// </summary>
        protected ResourceManager()
        {
            objects = new List<Resource>();
            hashTable = new Dictionary<string, Resource>();
            totalCacheSize = 8 * 1048576;  // 8mb
            manageFrequency = 4;
        }

        /// <summary>
        ///  以特定缓存大小创建一个资源管理器
        /// </summary>
        /// <param name="cacheSize">缓存大小</param>
        protected ResourceManager(int cacheSize)
        {
            objects = new List<Resource>();
            hashTable = new Dictionary<string, Resource>();
            totalCacheSize = cacheSize;
            manageFrequency = 4;
        }


        protected ResourceManager(int cacheSize, int manageFreq)
        {
            objects = new List<Resource>();
            hashTable = new Dictionary<string, Resource>();
            totalCacheSize = cacheSize;
            manageFrequency = manageFreq;
        }

        /// <summary>
        ///  获取或设置该资源管理器是否允许动态加载/释放
        /// </summary>
        public bool AllowDynamicLoading
        {
            get { return allowDynLd; }
            set { allowDynLd = value; }
        }

        /// <summary>
        ///  获取或设置最大允许的缓存大小
        /// </summary>
        public int TotalCacheSize
        {
            get { return totalCacheSize; }
            set
            {
                if (value < totalCacheSize)
                {
                    totalCacheSize = value;
                    Manage();
                }
                else
                {
                    totalCacheSize = value;
                }
            }
        }

        /// <summary>
        ///  获取已经使用的缓存大小
        /// </summary>
        public int UsedCacheSize
        {
            get { return curUsedCache; }
        }

        #region ICachedObjectManager 成员

        /// <summary>
        ///  比较器
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int Comparison(Resource a, Resource b)
        {
            return a.UseFrequency.CompareTo(b.UseFrequency);
        }

        /// <summary>
        ///  管理资源，将不常使用的销毁掉以释放内存
        /// </summary>
        public void Manage()
        {
            if (allowDynLd)
            {
                manageTimes++;

                if (manageTimes >= manageFrequency)
                {
                    manageTimes = 0;
                    if (curUsedCache > totalCacheSize)
                    {
                        objects.Sort(Comparison);

                        int oc = objects.Count;
                        int k = oc - 1;
                        while (curUsedCache > totalCacheSize && k >= 0)
                        {
                            if (objects[k].State == ResourceState.Loaded && objects[k].IsUnloadable)
                            {
                                objects[k].Unload();
                            }
                            k--;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  提示资源管理器一个资源已经加载
        /// </summary>
        /// <param name="res">资源</param>
        public void NotifyResourceLoaded(Resource res)
        {
            if (allowDynLd)
            {
                curUsedCache += res.GetSize();
            }
        }

        /// <summary>
        ///  提示资源管理器一个资源已经释放
        /// </summary>
        /// <param name="res">资源</param>
        public void NotifyResourceUnloaded(Resource res)
        {
            if (allowDynLd)
            {
                curUsedCache -= res.GetSize();
            }
        }

        /// <summary>
        ///  提示资源管理器一个资源已经销毁
        /// </summary>
        /// <param name="res">资源</param>
        public void NotifyResourceFinalizing(Resource res)
        {
            hashTable.Remove(res.HashString);
            objects.Remove(res);
        }

        #endregion


        /// <summary>
        ///  提示资源管理器已经创建了一个新资源，这个资源在创建时不考虑先前创建的资源
        /// </summary>
        /// <param name="res"></param>
        protected void NewCached(Resource res, CacheType ctype)
        {
            objects.Add(res);

            if (!res.AllowDynamicLoading)
            {
                allowDynLd = false;
            }

            int size = res.GetSize();

            CacheMemory cm;
            if (size == 0)
            {
                cm = Cache.Instance.Allocate();
            }
            else
            {
                if (ctype == CacheType.Dynamic)
                {
                    cm = Cache.Instance.Allocate(CacheType.Dynamic, size);
                }
                else
                {
                    cm = Cache.Instance.Allocate(CacheType.Static, size);
                }
            }
            res.SetCache(cm);
        }

        /// <summary>
        ///  提示资源管理器已经创建了一个新资源，将它放入管理范围
        /// </summary>
        /// <param name="res"></param>
        protected void NewResource(Resource res, CacheType ctype)
        {
            hashTable.Add(res.HashString, res);
            objects.Add(res);

            if (!res.AllowDynamicLoading)
            {
                allowDynLd = false;
            }

            int size = res.GetSize();

            CacheMemory cm;
            if (size == 0)
            {
                cm = Cache.Instance.Allocate();
            }
            else
            {
                if (ctype == CacheType.Dynamic)
                {
                    cm = Cache.Instance.Allocate(CacheType.Dynamic, size);
                }
                else
                {
                    EngineConsole.Instance.Write("正在为资源" + res.HashString + "分为静态缓存空间。");
                    cm = Cache.Instance.Allocate(CacheType.Static, size);
                }
            }
            res.SetCache(cm);
        }

        /// <summary>
        ///  销毁一个资源对象
        ///  即使该资源不受管理也照常销毁
        /// </summary>
        /// <param name="res"></param>
        protected void DestoryResource(Resource res)
        {
            if (res == null)
            {
                throw new ArgumentNullException("res");
            }


            if (!res.IsUnmamaged)
            {
                Cache.Instance.Release(res.GetCache());
                res.ResetCache();

                res.Dispose();
            }
            else
            {
                if (res.ResourceEntity != null)
                {
                    res.ResourceEntity.ReferenceCount--;
                    if (res.ResourceEntity.ReferenceCount == 0)
                    {
                        if (res.ResourceEntity.IsResourceEntity)
                        {
                            hashTable.Remove(res.ResourceEntity.HashString);
                            objects.Remove(res.ResourceEntity);
                        }
                        DestoryResource(res.ResourceEntity);
                    }
                }
                //res.Dispose(false);
            }
        }

        /// <summary>
        ///  检查一个资源
        /// </summary>
        /// <param name="hashcode">资源的哈希代码</param>
        /// <returns>如果找到，则返回该资源。否则返回null</returns>
        public Resource Exists(string hashString)
        {
            Resource res;
            if (hashTable.TryGetValue(hashString, out res))
            {
                return res;
            }
            return null;
        }

        
    }

}
