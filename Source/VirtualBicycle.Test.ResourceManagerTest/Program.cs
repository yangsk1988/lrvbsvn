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

            handle4.Resource.Visit();

            TestResource res = handle3.Resource;
            res.Visit();

            int lastTick = Environment.TickCount;
            while (res.State != ResourceState.Loaded) 
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步用时：{0}", (Environment.TickCount - lastTick).ToString());

            res = handle2.Resource;
            res.Visit();
            lastTick = Environment.TickCount;
            while (res.State != ResourceState.Loaded)
            {
                Thread.Sleep(10);
            }
            Console.WriteLine("异步用时：{0}", (Environment.TickCount - lastTick).ToString());


            for (int i = 0; i < 100; i++)
            {
                handle2.Resource.Visit();
                Thread.Sleep(100);
            }

            Console.WriteLine("代数：{0}", handle2.Resource.Generation);
            Console.WriteLine("代数：{0}", handle.Resource.Generation);
          
            Thread.Sleep(100);
            handle.Resource.Visit();

            Console.ReadKey();
        }
    }
}
