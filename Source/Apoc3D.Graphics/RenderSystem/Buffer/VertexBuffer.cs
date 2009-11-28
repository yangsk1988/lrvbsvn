using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.Graphics
{
    /// <summary>
    ///  表示顶点缓冲
    /// </summary>
    public abstract class VertexBuffer : HardwareBuffer
    {
        protected VertexBuffer(int vertexSize, int vertexCount, BufferUsage usage, bool useSysMem)
            : base(usage, vertexSize * vertexCount, useSysMem)
        {
            VertexSize = vertexSize;
            VertexCount = vertexCount;

        }


        public int VertexSize
        {
            get;
            private set;
        }

        public int VertexCount
        {
            get;
            private set;
        }



        //protected override IntPtr @lock(int offset, int size, LockMode mode)
        //{

        //}

        //protected override void unlock()
        //{
            
        //}

        //public override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {

        //    }

        //}
    }
}
