using System;
using System.Collections.Generic;
using System.Text;
using D3D = SlimDX.Direct3D9;

namespace Apoc3D.Graphics.D3D9
{
    internal class D3D9StateBlock : StateBlock
    {
        D3D.StateBlock block;

        public D3D9StateBlock(D3D9RenderSystem rs, StateBlockType type)
            : base(rs, type)
        {
            block = new SlimDX.Direct3D9.StateBlock(rs.D3DDevice, D3D9Utils.ConvertEnum(type));
        }

        public override void Capture()
        {
            block.Capture();
        }

        public override void Apply()
        {
            block.Apply();
        }
    }
}
