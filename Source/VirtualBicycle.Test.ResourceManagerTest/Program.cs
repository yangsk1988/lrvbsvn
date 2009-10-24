using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VirtualBicycle.Core;

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
            ResourceHandle<TestResource> handle3 = mgr.CreateInstance("a");
            ResourceHandle<TestResource> handle4 = mgr.CreateInstance("a");

            handle.Resource.Visit();

            Console.ReadKey();
        }
    }
}
