using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Core;
using System.IO;

namespace VirtualBicycle.Test.ResourceManagerTest
{
    class TestResource : Resource
    {
        int[] resource;

        public TestResource(TestResourceManager mgr, string name)
            : base(mgr, name)
        {

        }

        protected override void ReadCacheData(Stream stream)
        {
            
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

        public ResourceRef<TestResource> CreateInstance(string name)
        {
            Resource retrived = base.Exists(name);
            if (retrived == null)
            {
                TestResource test = new TestResource(this, name);
                retrived = test;
                base.NotifyNewResource(test, CacheType.Static);
            }
            return new ResourceRef<TestResource>((TestResource)retrived);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
