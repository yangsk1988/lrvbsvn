using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.Core;
using System.Threading;

namespace VirtualBicycle.Test.ResourceManagerTest
{
    class TestResource : Resource
    {
        int[] resource;

        public TestResource(TestResourceManager mgr, string name)
            : base(mgr, name)
        {

        }

        public void Visit() { }

        protected override void ReadCacheData(Stream stream)
        {
            resource = new int[128];
        }

        protected override void WriteCacheData(Stream stream)
        {

        }

        public override int GetSize()
        {
            return sizeof(int) * 128;
        }

        protected override void load()
        {
            resource = new int[128];
        }

        protected override void unload()
        {
            resource = null;
        }
    }

    class TestResourceManager : ResourceManager
    {
        public TestResourceManager(int size)
            : base(size)
        {
        }

        public ResourceHandle<TestResource> CreateInstance(string name)
        {
            Resource retrived = base.Exists(name);
            if (retrived == null)
            {
                TestResource test = new TestResource(this, name);
                retrived = test;
                base.NotifyResourceNew(test, CacheType.None);
            }
            return new ResourceHandle<TestResource>((TestResource)retrived);
        }
    }

    class Program
    {
        static void StandardScene()
        {
            TestResourceManager mgr = new TestResourceManager(512 * 3);

            Console.WriteLine("总管理器缓存大小{0}", mgr.TotalCacheSize);

            ResourceHandle<TestResource> handle = mgr.CreateInstance("a");
            ResourceHandle<TestResource> handle2 = mgr.CreateInstance("b");
            ResourceHandle<TestResource> handle3 = mgr.CreateInstance("c");
            ResourceHandle<TestResource> handle4 = mgr.CreateInstance("d");


            int lastTick = Environment.TickCount;
            TestResource res4 = handle4;
            while (res4.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步加载用时{0}：{1}", res4.HashString, (Environment.TickCount - lastTick).ToString());

            lastTick = Environment.TickCount;
            TestResource res2 = handle2;
            while (res2.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步加载用时{0}：{1}", res2.HashString, (Environment.TickCount - lastTick).ToString());

            lastTick = Environment.TickCount;
            TestResource res3 = handle3;
            while (res3.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步加载用时{0}：{1}", res3.HashString, (Environment.TickCount - lastTick).ToString());
            Console.WriteLine("加载四个资源，每个{0}字节", res4.GetSize());


            Console.WriteLine("开始访问测试...");
            Console.WriteLine("开始访问测试，访问资源{0}", res2.HashString);

            const int maxTest = 650;
            for (int i = 0; i < maxTest; i++)
            {
                handle2.Resource.Visit();
                Thread.Sleep(100);

                if (i % 10 == 0)
                {
                    Console.WriteLine("已完成：{0:P}", (float)i / maxTest);
                }
            }

            Console.WriteLine(" 资源{0}：{1}，代数{2}", res2.HashString, res2.State, res2.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res3.HashString, res3.State, res3.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res4.HashString, res4.State, res4.Generation);

            Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);


            Console.WriteLine("按任意键进行过缓存测试。");


            Console.ReadKey();

            Thread.Sleep(100);

            lastTick = Environment.TickCount;

            TestResource res1 = handle;
            while (!mgr.IsIdle)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步加载用时{0}：{1}", res1.HashString, (Environment.TickCount - lastTick).ToString());

            while (mgr.UsedCacheSize > mgr.TotalCacheSize)
            {
                Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);
                Thread.Sleep(1000);
            }
            Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);

            Console.WriteLine("资源状态：");

            Console.WriteLine(" 资源{0}：{1}，代数{2}", res1.HashString, res1.State, res1.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res2.HashString, res2.State, res2.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res3.HashString, res3.State, res3.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res4.HashString, res4.State, res4.Generation);

            Console.ReadKey();
        }
        static void ForcedAsync()
        {
            List<TestResource> list = new List<TestResource>(1000);
            List<ResourceHandle<TestResource>> list2 = new List<ResourceHandle<TestResource>>(1000);
            TestResourceManager mgr = new TestResourceManager(10240);

            Random rnd = new Random();
            const int maxTest = 650;

            for (int i = 0; i < maxTest; i++)
            {
                ResourceHandle<TestResource> handle = mgr.CreateInstance(rnd.NextDouble().ToString());
                list2.Add(handle);
                list.Add(handle);
                Thread.Sleep(100);

                if (i % 10 == 0)
                {
                    Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);
                    Console.WriteLine("已完成：{0:P}", (float)i / maxTest);
                }
            }
            while (mgr.UsedCacheSize > mgr.TotalCacheSize)
            {
                Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);
                Thread.Sleep(1000);
            }
            Console.WriteLine("已使用管理器缓冲大小：{0}", mgr.UsedCacheSize);
            Console.ReadKey();

            for (int i = 0; i < list.Count; i++) 
            {
                Console.Write("{0}|{1}", list[i].State.ToString().Substring(0, 1), list[i].Generation);
                if (i % 5 == 0)
                {
                    Console.WriteLine();
                }
                else 
                {
                    Console.Write(' ');
                    Console.Write(' ');
                }
            }

            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            ForcedAsync();
        }
    }
}
