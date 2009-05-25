using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using SlimDX;
using VirtualBicycle.Collections;
using VirtualBicycle.Graphics;

namespace VirtualBicycle.Scene
{
    public struct InstanceData 
    {
        public Vector3 Row1;
        public Vector3 Row2;
        public Vector3 Row3;
        public Vector3 Row4;

        public static int Size 
        {
            get { return Vector3.SizeInBytes * 4; }
        }

    }
    public  class Instancing:UnmanagedResource
    {
        public const int MaxInstances = 25;

        Device device;

        VertexBuffer instanceData;

        Dictionary<VertexDeclaration, VertexDeclaration> declBuffer;
        VertexElement[] idElements;

        public Instancing(Device device)
        {
            this.device = device;

            this.declBuffer = new Dictionary<VertexDeclaration, VertexDeclaration>();

            LoadUnmanagedResources();

            this.idElements = new VertexElement[4];
            this.idElements[0] = new VertexElement(1, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 8);
            this.idElements[1] = new VertexElement(1, (short)Vector3.SizeInBytes, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 9);
            this.idElements[2] = new VertexElement(1, (short)(Vector3.SizeInBytes * 2), DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 10);
            this.idElements[3] = new VertexElement(1, (short)(Vector3.SizeInBytes * 3), DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 11);
        }

        public unsafe int Setup(FastList<RenderOperation> opList, int index)
        {
            InstanceData* dst = (InstanceData*)instanceData.Lock(0, 0, LockFlags.None).DataPointer;

            int count = 0;
            for (int i = index; i < opList.Count && count < MaxInstances; i++)
            {
                dst->Row1 = new Vector3(opList[i].Transformation.M11, opList[i].Transformation.M12, opList[i].Transformation.M13);
                dst->Row2 = new Vector3(opList[i].Transformation.M21, opList[i].Transformation.M22, opList[i].Transformation.M23);
                dst->Row3 = new Vector3(opList[i].Transformation.M31, opList[i].Transformation.M32, opList[i].Transformation.M33);
                dst->Row4 = new Vector3(opList[i].Transformation.M41, opList[i].Transformation.M42, opList[i].Transformation.M43);

                dst++;
                count++;
            }

            instanceData.Unlock();

            device.SetStreamSource(1, instanceData, 0, InstanceData.Size);
            device.SetStreamSourceFrequency(1, 1, StreamSource.InstanceData);

            return count;
        }

        public VertexDeclaration GetInstancingDecl(VertexDeclaration decl)
        {
            VertexDeclaration idecl;
            if (!declBuffer.TryGetValue(decl, out idecl)) 
            {
                VertexElement[] elems1 = decl.Elements;
                VertexElement[] elems2 = new VertexElement[elems1.Length + idElements.Length];

                Array.Copy(elems1, elems2, elems1.Length - 1);
                Array.Copy(idElements, 0, elems2, elems1.Length - 1, idElements.Length);
                elems2[elems2.Length - 1] = VertexElement.VertexDeclarationEnd;
                
                idecl = new VertexDeclaration(device, elems2);

                declBuffer.Add(decl, idecl);
            }
            return idecl;
        }

        protected override void loadUnmanagedResources()
        {
            this.instanceData = new VertexBuffer(device, MaxInstances * InstanceData.Size, Usage.Dynamic, VertexFormat.None, Pool.Default);
        }

        protected override void unloadUnmanagedResources()
        {
            this.instanceData.Dispose();
            this.instanceData = null;
        }
    }
}
