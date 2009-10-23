using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;

using D3D = SlimDX.Direct3D9;

namespace VirtualBicycle.Graphics.D3D9
{
    internal class D3D9IndexBuffer : IndexBuffer
    {
        D3D.IndexBuffer indexBuffer;

        public D3D9IndexBuffer(D3D9RenderSystem rs, IndexBufferType type, int count, BufferUsage usage, bool useSysMem)
            : base(type, count, usage, useSysMem)
        {
            D3D.Pool d3dPool = useSysMem ? D3D.Pool.SystemMemory :
                // If not system mem, use managed pool UNLESS buffer is discardable
                // if discardable, keeping the software backing is expensive
                ((usage & BufferUsage.Discardable) != 0) ? D3D.Pool.Default : D3D.Pool.Managed;

            indexBuffer = new D3D.IndexBuffer(rs.D3DDevice, base.IndexSize * count, D3D9Utils.ConvertEnum(usage), d3dPool, type == IndexBufferType.Bit16);

        }

        protected override IntPtr @lock(int offset, int size, LockMode mode)
        {
            return indexBuffer.Lock(offset, size, D3D9Utils.ConvertEnum(mode, Usage)).DataPointer;
        }

        protected override void unlock()
        {
            indexBuffer.Unlock();
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                indexBuffer.Dispose();
            }
            indexBuffer = null;
        }

        internal D3D.IndexBuffer D3DIndexBuffer 
        {
            get { return indexBuffer; }
        }
    }
}
