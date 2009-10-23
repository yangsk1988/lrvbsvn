using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.RenderSystem;

namespace VirtualBicycle.Collections
{
    public class SamplerStateCollection : CollectionBase<SamplerState>
    {
        public SamplerStateCollection(SamplerState[] states)
            : base(states.Length)
        {
            Add(states);
        }
    }
}
