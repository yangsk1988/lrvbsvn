using System;
using System.Collections.Generic;
using System.Text;

using SlimDX.Direct3D9;
using D3D = SlimDX.Direct3D9;

namespace Apoc3D.Graphics.D3D9
{
    internal sealed class D3D9VertexBuffer : VertexBuffer
    {
        D3D.VertexBuffer vertexBuffer;

        public D3D9VertexBuffer(D3D9RenderSystem rs, int vertexCount, int vertexSize, BufferUsage usage, bool useSysMem)
            : base(vertexSize, vertexCount, usage, useSysMem)
        {
            Usage usage2 = D3D9Utils.ConvertEnum(usage);

            D3D.Pool d3dPool = useSysMem ? D3D.Pool.SystemMemory :
                ((usage & BufferUsage.Discardable) != 0) ? D3D.Pool.Default : D3D.Pool.Managed;


            vertexBuffer = new D3D.VertexBuffer(rs.D3DDevice, vertexCount * vertexSize, usage2, D3D.VertexFormat.Position, d3dPool);
        }


        protected override IntPtr @lock(int offset, int size, LockMode mode)
        {
            return vertexBuffer.Lock(offset, size, D3D9Utils.ConvertEnum(mode, Usage)).DataPointer;
        }

        protected override void unlock()
        {
            vertexBuffer.Unlock();
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                vertexBuffer.Dispose();
            }
            vertexBuffer = null;
        }

        internal D3D.VertexBuffer D3DVertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }
    }
}
