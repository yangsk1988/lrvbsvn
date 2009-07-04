using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Graphics;
using VirtualBicycle.IO;
using PM = VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel
{
    public class CollisionMesh : VirtualBicycle.Core.Resource
    {
        OptimizedBvh bvhTree;

        int sizeInBytes;

        Model model;
        CollisionMesh refMesh;
        TriangleIndexVertexArray meshData;

        Matrix trans;

        public TriangleIndexVertexArray MeshData
        {
            get
            {
                if (IsResourceEntity)
                {
                    Use();
                    return meshData;
                }
                return refMesh.MeshData;
            }
        }
        public OptimizedBvh BvhTree
        {
            get
            {
                if (IsResourceEntity)
                {
                    Use();
                    return bvhTree;
                }
                return refMesh.BvhTree;
            }
        }

        static string GetMatrixString(ref Matrix m)
        {
            return m.M11.ToString() + " " + m.M12.ToString() + " " + m.M13.ToString() +
                m.M21.ToString() + " " + m.M22.ToString() + " " + m.M23.ToString() +
                m.M31.ToString() + " " + m.M32.ToString() + " " + m.M33.ToString();
        }

        public static string GetHashString(Model model, Matrix trans)
        {
            if (model.ResourceEntity != null)
            {
                return "[CollisionMesh Shape]" + model.ResourceEntity.HashString + GetMatrixString(ref trans);
            }
            return "[CollisionMesh Shape]" + model.GetHashCode().ToString() + GetMatrixString(ref trans);
        }
        //public static string GetHashString(Resource parentRes, GameMesh mesh, Matrix trans)
        //{
        //    return "[CollisionMesh Shape]" + parentRes.HashString + mesh.GetHashCode().ToString() + GetMatrixString(ref trans);
        //}

        public CollisionMesh(CollisionMesh mesh)
            : base(CollisionMeshManager.Instance, mesh)
        {
            this.refMesh = mesh;
        }

        public CollisionMesh(Model model, Matrix trans)
            : base(CollisionMeshManager.Instance, GetHashString(model, trans))
        {
            this.model = model;
            //this.parentRes = parentRes;

            this.trans = trans;
        }
        

        public unsafe override void ReadCacheData(Stream stream)
        {
            bvhTree = new OptimizedBvh();

            ContentBinaryReader br = new ContentBinaryReader(stream);
            bvhTree.Load(br);
            br.Close();
        }
        public override void WriteCacheData(Stream stream)
        {
            Use();

            ContentBinaryWriter bw = new ContentBinaryWriter(stream);
            bvhTree.Save(bw);
            bw.Close();
        }


        public override int GetSize()
        {
            return sizeInBytes;
        }

        unsafe void BuildCollisionMesh(Model model, out PM.Vector3[] resultVertices, out int[] resultIndices)
        {
            GameMesh[] meshes = model.Entities;

            List<PM.Vector3[]> positions = new List<PM.Vector3[]>();
            List<int[]> idxList = new List<int[]>();

            int vtxCount = 0;
            int idxCount = 0;

            int indexOffset = 0;
            for (int i = 0; i < meshes.Length; i++)
            {
                GameMesh mesh = meshes[i];

                Matrix localT = model.TransformAnim.GetTransform(i) * this.trans;

                int vertexCount = mesh.VertexCount;
                int vertexSize = mesh.VertexSize;

                int[] indices = mesh.GetIndices();
                for (int j = 0; j < indices.Length; j++)
                {
                    indices[j] += indexOffset;
                }

                PM.Vector3[] vertices = new PM.Vector3[vertexCount];

                int ofs = 0;
                VertexBuffer vb = mesh.VertexBuffer;

                byte* src = (byte*)vb.Lock(0, 0, LockFlags.ReadOnly).DataPointer.ToPointer();

                for (int j = 0; j < vertexCount; j++)
                {
                    Vector3 srcVtx = *((PM.Vector3*)(src + ofs));

                    Vector3 result;
                    Vector3.TransformCoordinate(ref srcVtx, ref localT, out result);

                    vertices[j] = result;

                    ofs += vertexSize;
                }

                positions.Add(vertices);
                idxList.Add(indices);

                vb.Unlock();

                indexOffset += vertices.Length;

                vtxCount += vertexCount;
                idxCount += indices.Length;
            }


            resultVertices = new PM.Vector3[vtxCount];
            resultIndices = new int[idxCount];

            int dstVtxIdx = 0;
            int dstIdxIdx = 0;
            for (int i = 0; i < positions.Count; i++)
            {
                Array.Copy(positions[i], 0, resultVertices, dstVtxIdx, positions[i].Length);
                Array.Copy(idxList[i], 0, resultIndices, dstIdxIdx, idxList[i].Length);

                dstVtxIdx += positions[i].Length;
                dstIdxIdx += idxList[i].Length;
            }
        }

        protected unsafe override void load()
        {
            if (model != null)
            {
                model.Use();
            }

            PM.Vector3[] vertices;
            int[] indices;

            BuildCollisionMesh(model, out vertices, out indices);

            sizeInBytes = vertices.Length * sizeof(Vector3) + indices.Length * sizeof(int);

            meshData = new TriangleIndexVertexArray(indices.Length / 3, indices, sizeof(int) * 3, vertices.Length, vertices, sizeof(float) * 3);
            bvhTree = new OptimizedBvh();
            bvhTree.Build(meshData);

        }

        protected override void unload()
        {
            bvhTree = null;
        }
    }
}
