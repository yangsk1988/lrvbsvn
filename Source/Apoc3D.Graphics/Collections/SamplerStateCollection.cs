using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;

namespace Apoc3D.Collections
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
