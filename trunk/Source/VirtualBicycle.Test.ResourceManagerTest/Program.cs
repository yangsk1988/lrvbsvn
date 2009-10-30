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
        public TestResourceManager()
            : base(1024)
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
        static void Main(string[] args)
        {
            TestResourceManager mgr = new TestResourceManager();

            ResourceHandle<TestResource> handle = mgr.CreateInstance("a");
            ResourceHandle<TestResource> handle2 = mgr.CreateInstance("b");
            ResourceHandle<TestResource> handle3 = mgr.CreateInstance("c");
            ResourceHandle<TestResource> handle4 = mgr.CreateInstance("d");

            TestResource res4 = handle4;

            int lastTick = Environment.TickCount;
            TestResource res2 = handle2;
            while (res2.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步用时2：{0}", (Environment.TickCount - lastTick).ToString());
          
            lastTick = Environment.TickCount;
            TestResource res3 = handle3;
            while (res3.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步用时3：{0}", (Environment.TickCount - lastTick).ToString());

            Console.WriteLine("开始访问测试...");

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

            Console.WriteLine("管理器缓冲大小：{0}", mgr.UsedCacheSize);

            Console.WriteLine("按任意键继续测试");
            Console.ReadKey();

            Thread.Sleep(100);

            lastTick = Environment.TickCount;

            TestResource res1 = handle;

            while (!mgr.IsIdle)
            {
                Thread.Sleep(10);
            } 
            Console.WriteLine("异步用时1：{0}", (Environment.TickCount - lastTick).ToString());
            Console.WriteLine("管理器缓冲大小：{0}", mgr.UsedCacheSize);

            Console.WriteLine("资源状态：");

            Console.WriteLine(" 资源{0}：{1}，代数{2}", res1.HashString, res1.State, res1.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res2.HashString, res2.State, res2.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res3.HashString, res3.State, res3.Generation);
            Console.WriteLine(" 资源{0}：{1}，代数{2}", res4.HashString, res4.State, res4.Generation);



            Console.ReadKey();
        }
    }
}
