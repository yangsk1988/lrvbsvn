﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using VirtualBicycle.Vfs;
using VirtualBicycle.Collections;

namespace VirtualBicycle.Core
{
    /// <summary>
    ///  资源分代管理，代数越小，资源使用越频繁
    /// </summary>
    class GenerationTable
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

        const int MaxGeneration = 4;

        ExistTable<Resource>[] gen;
        
        public GenerationTable()
        {
            gen = new ExistTable<Resource>[4];

            gen[0] = new ExistTable<Resource>();
            gen[1] = new ExistTable<Resource>();
            gen[2] = new ExistTable<Resource>();
            gen[3] = new ExistTable<Resource>();
        }

        public ExistTable<Resource> this[int index]
        {
            get
            {
                return gen[index];
            }
        }
    }

    /// <summary>
    ///  定义一个资源管理器，可以动态的进行资源加载与释放
    /// </summary>
    public abstract class ResourceManager
    {
        /// <summary>
        ///  受管理的资源哈希表
        /// </summary>
        Dictionary<string, Resource> hashTable;

        /// <summary>
        ///  受管理的资源的列表
        /// </summary>
        GenerationTable genTable = new GenerationTable();

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

        
        AsyncProcessor asyncProc = new AsyncProcessor();

        /// <summary>
        ///  以默认缓存大小创建一个资源管理器
        /// </summary>
        protected ResourceManager()
        {
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
            hashTable = new Dictionary<string, Resource>();
            totalCacheSize = cacheSize;
            manageFrequency = 4;
        }

        protected ResourceManager(int cacheSize, int manageFreq)
        {
            hashTable = new Dictionary<string, Resource>();
            totalCacheSize = cacheSize;
            manageFrequency = manageFreq;
        }

        internal GenerationTable Table
        {
            get { return genTable; }
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

        /// <summary>
        ///  管理资源，将不常使用的销毁掉以释放内存
        /// </summary>
        public void Manage()
        {
            manageTimes++;

            if (manageTimes >= manageFrequency)
            {
                manageTimes = 0;
                if (curUsedCache > totalCacheSize)
                {
                    int predictCSize = curUsedCache;

                    while (predictCSize > totalCacheSize)
                    {
                        for (int i = 3; i >= 2; i++)
                            foreach (Resource r in genTable[i])
                                if (r.State == ResourceState.Loaded && r.IsUnloadable)
                                {
                                    predictCSize -= r.GetSize();
                                    r.Unload();
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
            curUsedCache += res.GetSize();
            Manage();
        }

        /// <summary>
        ///  提示资源管理器一个资源已经释放
        /// </summary>
        /// <param name="res">资源</param>
        public void NotifyResourceUnloaded(Resource res)
        {
            curUsedCache -= res.GetSize();
        }

        /// <summary>
        ///  提示资源管理器一个资源已经销毁
        /// </summary>
        /// <param name="res">资源</param>
        public void NotifyResourceFinalizing(Resource res)
        {
            hashTable.Remove(res.HashString);
            if (res.Generation != -1) 
            {
                genTable[res.Generation].Remove(res);
            }
        }

        public void AddTask(ResourceOperation op) 
        {
            asyncProc.AddTask(op);
        }
        ///// <summary>
        /////  提示资源管理器已经创建了一个新资源，这个资源在创建时不考虑先前创建的资源
        ///// </summary>
        ///// <param name="res"></param>
        //protected void NotifyNewCached(Resource res, CacheType ctype)
        //{
        //    objects.Add(res);

        //    int size = res.GetSize();

        //    CacheMemory cm;
        //    if (size == 0)
        //    {
        //        cm = Cache.Instance.Allocate();
        //    }
        //    else
        //    {
        //        if (ctype == CacheType.Dynamic)
        //        {
        //            cm = Cache.Instance.Allocate(CacheType.Dynamic, size);
        //        }
        //        else
        //        {
        //            cm = Cache.Instance.Allocate(CacheType.Static, size);
        //        }
        //    }
        //    res.SetCache(cm);
        //}

        /// <summary>
        ///  提示资源管理器已经创建了一个新资源，将它放入管理范围
        /// </summary>
        /// <param name="res"></param>
        protected void NotifyResourceNew(Resource res, CacheType ctype)
        {
            hashTable.Add(res.HashString, res);
            if (res.Generation != -1)
            {
                genTable[res.Generation].Add(res);
            }

            if (ctype != CacheType.None)
            {
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