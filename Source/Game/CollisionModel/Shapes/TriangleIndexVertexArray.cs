/*
  Bullet for XNA Copyright (c) 2007-2009 Vsevolod Klementjev & Mikhail Pashnin http://www.codeplex.com/xnadevru
  Bullet original C++ version Copyright (c) 2003-2007 Erwin Coumans http://bulletphysics.com

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Shapes
{
    /// <summary>
    /// IndexedMesh indexes into existing vertex and index arrays, in a similar way OpenGL glDrawElements
    /// instead of the number of indices, we pass the number of triangles
    /// </summary>
    [Serializable]
    public struct IndexedMesh
    {
        private int _numTriangles;
        private int[] _triangleIndexBase;
        private int _triangleIndexStride;
        private int _numVertices;
        private Vector3[] _vertexBase;
        private int _vertexStride;

        public IndexedMesh(int numTriangleIndices, int[] triangleIndexBase, int triangleIndexStride, int numVertices, Vector3[] vertexBase, int vertexStride)
        {
            _numTriangles = numTriangleIndices;
            _triangleIndexBase = triangleIndexBase;
            _triangleIndexStride = triangleIndexStride;
            _vertexBase = vertexBase;
            _numVertices = numVertices;
            _vertexStride = vertexStride;
        }

        public IndexedMesh(int[] triangleIndexBase, Vector3[] vertexBase)
        {
            _numTriangles = triangleIndexBase.Length;
            _triangleIndexBase = triangleIndexBase;
            _triangleIndexStride = 32;
            _vertexBase = vertexBase;
            _numVertices = vertexBase.Length;
            _vertexStride = 24;
        }

        public int TriangleCount
        {
            get { return _numTriangles; }
            set { _numTriangles = value; }
        }
        public int[] TriangleIndexBase
        {
            get { return _triangleIndexBase; }
            set { _triangleIndexBase = value; }
        }
        public int TriangleIndexStride
        {
            get { return _triangleIndexStride; }
            set { _triangleIndexStride = value; }
        }
        public int VertexCount
        {
            get { return _numVertices; }
            set { _numVertices = value; }
        }
        public Vector3[] VertexBase
        {
            get { return _vertexBase; }
            set { _vertexBase = value; }
        }
        public int VertexStride
        {
            get { return _vertexStride; }
            set { _vertexStride = value; }
        }
    }

    /// <summary>
    /// TriangleIndexVertexArray allows to use multiple meshes, by indexing into existing triangle/index arrays.
    /// Additional meshes can be added using addIndexedMesh
    /// </summary>
    [Serializable]
    public class TriangleIndexVertexArray : StridingMeshInterface
    {
        private List<IndexedMesh> _indexedMeshes = new List<IndexedMesh>();

        public TriangleIndexVertexArray() { }

        public TriangleIndexVertexArray(int numTriangleIndices, int[] triangleIndexBase, int triangleIndexStride, int numVertices, Vector3[] vertexBase, int vertexStride)
        {
            IndexedMesh mesh = new IndexedMesh();
            mesh.TriangleCount = numTriangleIndices;
            mesh.TriangleIndexBase = triangleIndexBase;
            mesh.TriangleIndexStride = triangleIndexStride;
            mesh.VertexBase = vertexBase;
            mesh.VertexCount = numVertices;
            mesh.VertexStride = vertexStride;

            AddIndexedMesh(mesh);
        }

        public TriangleIndexVertexArray(int[] triangleIndexBase, Vector3[] vertexBase)
            : this(triangleIndexBase.Length, triangleIndexBase, sizeof(int) * 3, vertexBase.Length, vertexBase, sizeof(float) * 3)
        {
        }

        public List<IndexedMesh> IndexedMeshes
        {
            get { return _indexedMeshes; }
        }

        public void AddIndexedMesh(IndexedMesh indexedMesh)
        {
            _indexedMeshes.Add(indexedMesh);
        }

        public override void GetLockedVertexIndexBase(out List<Vector3> verts, out List<int> indicies, out int numfaces, int subpart)
        {
            verts = new List<Vector3>(_indexedMeshes[subpart].VertexBase);
            indicies = new List<int>(_indexedMeshes[subpart].TriangleIndexBase);
            numfaces = _indexedMeshes[subpart].TriangleCount;
        }

        public override void GetLockedReadOnlyVertexIndexBase(out List<Vector3> verts, out List<int> indicies, out int numfaces, int subpart)
        {
            GetLockedVertexIndexBase(out verts, out indicies, out numfaces, subpart);
        }

        public override void UnLockVertexBase(int subpart)
        {
        }

        public override void UnLockReadOnlyVertexBase(int subpart)
        {
        }

        public override int SubPartsCount()
        {
            return _indexedMeshes.Count;
        }

        public override void PreallocateVertices(int numverts)
        {
        }

        public override void PreallocateIndices(int numindices)
        {
        }
    }
}
