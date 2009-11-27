using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Collections
{
    public class PlatformCollection : CollectionBase<PlatformAPISupport>
    {
        public PlatformCollection(PlatformAPISupport[] items)
        {
            Add(items);
        }

        public new bool Contains(PlatformAPISupport item)
        {
            return base.Contains(item);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }
    }
}
