using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JigLibX.Geometry;
using JigLibX.Physics;
using VirtualBicycle.Graphics;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  表示静态物体
    /// </summary>
    public abstract class StaticModelObject : Entity
    {
        #region 字段

        bool isPhyBuilt;

        TriangleMesh cdMesh;

        #endregion

        #region 构造函数

        public StaticModelObject()
            : base(false)
        {
        }

        public StaticModelObject(bool hasSubObjects)
            : base(hasSubObjects)
        {

        }

        public StaticModelObject(Vector3 position, Quaternion orientation, bool hasSubObjects)
            : base(hasSubObjects)
        {
            base.position = position;
            base.orientation = orientation;

            base.UpdateTransform();
        }

        #endregion

       
        //public override bool IntersectsSelectionRay(ref Ray ray)
        //{
        //    if (RigidBody == null)
        //    {
        //        return base.IntersectsSelectionRay(ref ray);                       
        //    }

        //    BvhTriMeshResShape shape = RigidBody.CollisionShape as BvhTriMeshResShape;

        //    LineSegment line = new LineSegment(ray.Position, ray.Direction * 1000 + ray.Position);

        //    if (shape != null)
        //    {
        //        CollisionWorld.ClosestRayResultCallback cbk = new CollisionWorld.ClosestRayResultCallback(line.Start, line.End);

        //        PM.Matrix worldTocollisionObject = MathUtil.InvertMatrix(RigidBody.WorldTransform);

        //        PM.Vector3 rayFromLocal = PM.Vector3.TransformNormal(line.Start, worldTocollisionObject);
        //        PM.Vector3 rayToLocal = PM.Vector3.TransformNormal(line.End, worldTocollisionObject);

        //        BridgeTriangleRaycastCallback rcb = new BridgeTriangleRaycastCallback(rayFromLocal, rayToLocal, cbk, RigidBody, shape);
        //        rcb.HitFraction = cbk.ClosestHitFraction;

        //        PM.Vector3 rayAabbMinLocal = rayFromLocal;
        //        MathUtil.SetMin(ref rayAabbMinLocal, rayToLocal);
        //        PM.Vector3 rayAabbMaxLocal = rayFromLocal;
        //        MathUtil.SetMax(ref rayAabbMaxLocal, rayToLocal);

        //        shape.ProcessAllTriangles(rcb, rayAabbMinLocal, rayAabbMaxLocal);

        //        return (cbk.CollisionObject == RigidBody);
        //    }

        //    return false;
        //}

        #region 物理相关

        [Browsable(false)]
        public override bool HasPhysicsModel
        {
            get { return true; }
        }
        public override bool IntersectsSelectionRay(ref VirtualBicycle.MathLib.Ray ray)
        {
            if (cdMesh != null) 
            {
                float frac;
                Vector3 p1, p2;
                Segment seg = new Segment(ray.Position, ray.Direction * 1000);
                if (cdMesh.SegmentIntersect(out fac, out p1, out p2, seg)) 
                {
                    return true;
                }
            }
            return base.IntersectsSelectionRay(ref ray);
        }

        public unsafe override void BuildPhysicsModel(PhysicsSystem world)
        {
            if (!isPhyBuilt)
            {
                //UpdateTransform();

                //Matrix trans = Transformation;
                //trans.M41 = 0;
                //trans.M42 = 0;
                //trans.M43 = 0;

                //cdMesh = CollisionMeshManager.Instance.CreateInstance(ModelL0, trans);
                //BvhTriMeshResShape shape = new BvhTriMeshResShape(cdMesh);


                //motionState = new DefaultMotionState(Matrix.Translation(position));

                //RigidBody = new RigidBody(0, motionState, shape);
                //RigidBody.CollisionFlags |= CollisionOptions.StaticObject;

                //PM.Vector3 aabbMin;
                //PM.Vector3 aabbMax;
                //shape.GetAabb(PM.Matrix.Identity, out aabbMin, out aabbMax);

                //float rad = PM.Vector3.Distance(aabbMin, aabbMax) * 0.5f;

                //BoundingSphereOffset = 0.5f * (aabbMin + aabbMax);
                //BoundingSphere.Radius = rad;

                //if (world != null)
                //{
                //    world.AddRigidBody(RigidBody);
                //}
                isPhyBuilt = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //if (disposing)
            //{
            //    CollisionMeshManager.Instance.DestoryInstance(cdMesh);
            //}
            cdMesh = null;
        }
        #endregion
    }
}
