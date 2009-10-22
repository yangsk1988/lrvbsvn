using System;
using System.Collections.Generic;
using System.Text;
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Shapes
{
    [Serializable]
    public class BvhTriMeshResShape : TriangleMeshShape
    {
        //private bool _useQuantizedAabbCompression;
        private bool[] _pad = new bool[12];
        CollisionMesh collMesh;

        public BvhTriMeshResShape(CollisionMesh collMesh)
            : base(collMesh.MeshData)
        {
            this.collMesh = collMesh;
        }

        public OptimizedBvh OptimizedBvh
        {
            get { return collMesh.BvhTree; }
        }
        public bool UseQuantizedAabbCompression
        {
            get { return false; }
        }

        public override void ProcessAllTriangles(ITriangleCallback callback, Vector3 aabbMin, Vector3 aabbMax)
        {
            MyNodeOverlapCallback myNodeCallback = new MyNodeOverlapCallback(callback, MeshInterface);

            OptimizedBvh.ReportAabbOverlappingNodex(myNodeCallback, aabbMin, aabbMax);
        }

        public override string Name
        {
            get
            {
                return "BvhTriangleMeshRes";
            }
        }
    }
}
