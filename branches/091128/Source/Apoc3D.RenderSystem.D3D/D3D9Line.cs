using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Apoc3D.Media;
using D3D = SlimDX.Direct3D9;

namespace Apoc3D.Graphics.D3D9
{

    internal sealed unsafe class D3D9Line : Line
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct LineVertex
        {
            public Vector3 position;
            float dummy;
            public int diffuse;

            public LineVertex(Vector3 position, int color)
            {
                this.position = position;
                this.diffuse = color;
                dummy = 0;
            }

            public static D3D.VertexFormat Format
            {
                get { return D3D.VertexFormat.Position | D3D.VertexFormat.Diffuse; }
            }

            public static unsafe int Size
            {
                get { return sizeof(LineVertex); }
            }
        }

        unsafe class Entry
        {
            const int InitCapacity = 100;

            D3D.VertexBuffer bufferedVB;
            D3D.Device dev;

            int capacity;
            bool isLocked;

            LineVertex* dataPtr;

            public bool IsLocked
            {
                get { return isLocked; }
            }

            public int Capacity
            {
                get { return capacity; }
            }

            public Entry(D3D.Device dev)
            {
                this.dev = dev;
                Resize(InitCapacity);
            }

            public void Resize(int count)
            {
                bufferedVB = new D3D.VertexBuffer(dev, LineVertex.Size * count, D3D.Usage.Dynamic, LineVertex.Format, D3D.Pool.Default);
                this.capacity = count;
            }

            public void Lock()
            {
                if (!isLocked)
                {
                    dataPtr = (LineVertex*)bufferedVB.Lock(0, 0, D3D.LockFlags.None).DataPointer.ToPointer();

                    isLocked = true;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            public void Unlock()
            {
                if (isLocked)
                {
                    dataPtr = null;
                    isLocked = false;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            public void AddVertex(Vector2[] vtx, int color)
            {
                if (!isLocked)
                {
                    throw new InvalidOperationException();
                }

                int len = vtx.Length;
                if (capacity < vtx.Length)
                {
                    int newC = capacity * 2;
                    if (len > newC)
                    {
                        newC = len;
                    }
                    Resize(newC);
                }

                for (int i = 0; i < len; i++)
                {
                    (dataPtr + i)->position.X = vtx[i].X;
                    (dataPtr + i)->position.Y = vtx[i].Y;
                    (dataPtr + i)->position.Z = 0;

                    (dataPtr + i)->diffuse = color;
                }
            }
        }

        FastList<Entry> buffer;

        FastList<RenderOperation> opBuffer;

        D3D.Device device;

        int curIndex;

        public D3D9Line(D3D9RenderSystem rs)
            : base(rs)
        {
            //line = new D3D.Line(rs.D3DDevice);
            opBuffer = new FastList<RenderOperation>();
            buffer = new FastList<Entry>();
            device = rs.D3DDevice;
        }

        public override void Draw(Vector2[] vertices, ColorValue color)
        {
            Entry buf;
            if (curIndex >= buffer.Count)
            {
                buf = new Entry(device);
                buffer.Add(buf);
            }
            else
            {
                buf = buffer[curIndex];
            }

            buf.AddVertex(vertices, (int)color.PackedValue);
            curIndex++;
        }
        
        public override void Draw(Vector3[] vertices, Matrix transformation, ColorValue color)
        {
            Entry buf;
            if (curIndex >= buffer.Count)
            {
                buf = new Entry(device);
                buffer.Add(buf);
            }
            else
            {
                buf = buffer[curIndex];
            }



            curIndex++;
        }
        public override void DrawProj(Vector3[] vertices, Matrix transformation, ColorValue color)
        {
            Entry buf;
            if (curIndex >= buffer.Count)
            {
                buf = new Entry(device);
                buffer.Add(buf);
            }
            else
            {
                buf = buffer[curIndex];
            }

        }

        public override float Width
        {
            get;
            set;
        }

        public override void Begin()
        {
            curIndex = 0;
            opBuffer.FastClear();
        }

        public override void End()
        {
            
        }

        public override RenderOperation[] GetRenderOperation()
        {
            return opBuffer.Elements;
        }
    }
}
