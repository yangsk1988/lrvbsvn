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
using VirtualBicycle.CollisionModel.Broadphase;
using VirtualBicycle.CollisionModel.NarrowPhase;
using VirtualBicycle.CollisionModel.Shapes;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Dispatch
{
    [Serializable]
    public class CollisionWorld
    {
        private List<CollisionObject> _collisionObjects = new List<CollisionObject>();
        private IDispatcher _dispatcher;
        private OverlappingPairCache _broadphasePairCache;
        private bool _ownsDispatcher;
        private bool _ownsBroadphasePairCache;
        private DispatcherInfo _dispatchInfo = new DispatcherInfo();

        /// <summary>
        /// this constructor doesn't own the dispatcher and paircache/broadphase
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="pairCache"></param>
        public CollisionWorld(IDispatcher dispatcher, OverlappingPairCache pairCache)
        {
            _dispatcher = dispatcher;
            _broadphasePairCache = pairCache;
            _ownsDispatcher = false;
            _ownsBroadphasePairCache = false;
        }

        public DispatcherInfo DispatchInfo
        {
            get { return _dispatchInfo; }
            protected set { _dispatchInfo = value; }
        }
        public List<CollisionObject> CollisionObjects
        {
            get { return _collisionObjects; }
            protected set { _collisionObjects = value; }
        }
        public IBroadphase Broadphase
        {
            get { return _broadphasePairCache; }
        }
        public OverlappingPairCache BroadphasePairCache
        {
            get { return _broadphasePairCache; }
            protected set { _broadphasePairCache = value; }
        }
        public IDispatcher Dispatcher
        {
            get { return _dispatcher; }
            protected set { _dispatcher = value; }
        }
        public int CollisionObjectsCount
        {
            get { return _collisionObjects.Count; }
        }
        protected bool OwnsDispatcher
        {
            get { return _ownsDispatcher; }
            set { _ownsDispatcher = value; }
        }
        protected bool OwnsBroadphasePairCache
        {
            get { return _ownsBroadphasePairCache; }
            set { _ownsBroadphasePairCache = value; }
        }

        // rayTest performs a raycast on all objects in the btCollisionWorld, and calls the resultCallback
        // This allows for several queries: first hit, all hits, any hit, dependent on the value returned by the callback.
        public void RayTest(Vector3 rayFromWorld, Vector3 rayToWorld, RayResultCallback resultCallback, short collisionFilterMask)
        {
            Matrix rayFromTrans, rayToTrans;

            rayFromTrans = Matrix.Identity;
            rayFromTrans.Translation = rayFromWorld;

            rayToTrans = Matrix.Identity;
            rayToTrans.Translation = rayToWorld;

            // brute force go over all objects. Once there is a broadphase, use that, or
            // add a raycast against aabb first.

            foreach (CollisionObject collisionObject in _collisionObjects)
            {
                //only perform raycast if filterMask matches
                if (((short)collisionObject.Broadphase.CollisionFilterGroup & collisionFilterMask) != 0)
                {
                    Vector3 collisionObjectAabbMin, collisionObjectAabbMax;
                    collisionObject.CollisionShape.GetAabb(collisionObject.WorldTransform, out collisionObjectAabbMin, out collisionObjectAabbMax);

                    float hitLambda = 1f; //could use resultCallback.m_closestHitFraction, but needs testing
                    Vector3 hitNormal = new Vector3();

                    if (MathUtil.RayAabb(rayFromWorld, rayToWorld, collisionObjectAabbMin, collisionObjectAabbMax, hitLambda, hitNormal))
                    {
                        RayTestSingle(rayFromTrans, rayToTrans,
                            collisionObject, collisionObject.CollisionShape,
                                 collisionObject.WorldTransform, resultCallback);

                    }
                }
            }
        }

        public static void RayTestSingle(Matrix rayFromTrans, Matrix rayToTrans,
                          CollisionObject collisionObject,
                          CollisionShape collisionShape,
                          Matrix colObjWorldTransform,
                          RayResultCallback resultCallback)
        {
            RayTestSingle(rayFromTrans, rayToTrans, collisionObject, collisionShape, colObjWorldTransform, resultCallback, -1);
        }

        // rayTestSingle performs a raycast call and calls the resultCallback. It is used internally by rayTest.
        // In a future implementation, we consider moving the ray test as a virtual method in CollisionShape.
        // This allows more customization.
        public static void RayTestSingle(Matrix rayFromTrans, Matrix rayToTrans,
                          CollisionObject collisionObject,
                          CollisionShape collisionShape,
                          Matrix colObjWorldTransform,
                          RayResultCallback resultCallback, short collisionFilterMask)
        {
            SphereShape pointShape = new SphereShape(0.0f);
            pointShape.Margin = 0;

            ObjectQuerySingle(pointShape, rayFromTrans, rayToTrans, collisionObject, collisionShape, colObjWorldTransform, resultCallback, collisionFilterMask);
        }

        public static void ObjectQuerySingle(ConvexShape castShape, Matrix rayFromTrans, Matrix rayToTrans,
                          CollisionObject collisionObject,
                          CollisionShape collisionShape,
                          Matrix colObjWorldTransform,
                          RayResultCallback resultCallback)
        {
            ObjectQuerySingle(castShape, rayFromTrans, rayToTrans, collisionObject, collisionShape, colObjWorldTransform, resultCallback, -1);
        }

        // objectQuerySingle performs a collision detection query and calls the resultCallback. It is used internally by rayTest.
        public static void ObjectQuerySingle(ConvexShape castShape, Matrix rayFromTrans, Matrix rayToTrans,
                          CollisionObject collisionObject,
                          CollisionShape collisionShape,
                          Matrix colObjWorldTransform,
                          RayResultCallback resultCallback, short collisionFilterMask)
        {
            if (collisionShape.IsConvex)
            {
                CastResult castResult = new CastResult();
                castResult.Fraction = 1f;//??

                ConvexShape convexShape = collisionShape as ConvexShape;
                VoronoiSimplexSolver simplexSolver = new VoronoiSimplexSolver();
                SubsimplexConvexCast convexCaster = new SubsimplexConvexCast(castShape, convexShape, simplexSolver);

                if (convexCaster.CalcTimeOfImpact(rayFromTrans, rayToTrans, colObjWorldTransform, colObjWorldTransform, castResult))
                {
                    //add hit
                    if (castResult.Normal.LengthSquared() > 0.0001f)
                    {
                        castResult.Normal.Normalize();
                        if (castResult.Fraction < resultCallback.ClosestHitFraction)
                        {

                            CollisionWorld.LocalRayResult localRayResult = new LocalRayResult
                                (
                                    collisionObject,
                                    new LocalShapeInfo(),
                                    castResult.Normal,
                                    castResult.Fraction
                                );

                            bool normalInWorldSpace = true;
                            resultCallback.AddSingleResult(localRayResult, normalInWorldSpace);
                        }
                    }
                }
                else
                {
                    if (collisionShape.IsConcave)
                    {

                        TriangleMeshShape triangleMesh = collisionShape as TriangleMeshShape;

                        Matrix worldTocollisionObject = MathUtil.InvertMatrix(colObjWorldTransform);

                        Vector3 rayFromLocal = Vector3.TransformNormal(rayFromTrans.Translation, worldTocollisionObject);
                        Vector3 rayToLocal = Vector3.TransformNormal(rayToTrans.Translation, worldTocollisionObject);

                        BridgeTriangleRaycastCallback rcb = new BridgeTriangleRaycastCallback(rayFromLocal, rayToLocal, resultCallback, collisionObject, triangleMesh);
                        rcb.HitFraction = resultCallback.ClosestHitFraction;

                        Vector3 rayAabbMinLocal = rayFromLocal;
                        MathUtil.SetMin(ref rayAabbMinLocal, rayToLocal);
                        Vector3 rayAabbMaxLocal = rayFromLocal;
                        MathUtil.SetMax(ref rayAabbMaxLocal, rayToLocal);

                        triangleMesh.ProcessAllTriangles(rcb, rayAabbMinLocal, rayAabbMaxLocal);
                    }
                    else
                    {
                        //todo: use AABB tree or other BVH acceleration structure!
                        if (collisionShape.IsCompound)
                        {
                            CompoundShape compoundShape = collisionShape as CompoundShape;
                            for (int i = 0; i < compoundShape.ChildShapeCount; i++)
                            {
                                Matrix childTrans = compoundShape.GetChildTransform(i);
                                CollisionShape childCollisionShape = compoundShape.GetChildShape(i);
                                Matrix childWorldTrans = colObjWorldTransform * childTrans;
                                ObjectQuerySingle(castShape, rayFromTrans, rayToTrans,
                                    collisionObject,
                                    childCollisionShape,
                                    childWorldTrans,
                                    resultCallback, collisionFilterMask);
                            }
                        }
                    }
                }
            }
        }


        public void AddCollisionObject(CollisionObject collisionObject, BroadphaseProxy.CollisionFilterGroups collisionFilterGroup, BroadphaseProxy.CollisionFilterGroups collisionFilterMask)
        {
            //check that the object isn't already added
            if (!_collisionObjects.Contains(collisionObject))
            {
                _collisionObjects.Add(collisionObject);

                //calculate new AABB
                Matrix trans = collisionObject.WorldTransform;

                Vector3 minAabb;
                Vector3 maxAabb;
                collisionObject.CollisionShape.GetAabb(trans, out minAabb, out maxAabb);

                BroadphaseNativeTypes type = collisionObject.CollisionShape.ShapeType;
                collisionObject.Broadphase = Broadphase.CreateProxy(
                    minAabb,
                    maxAabb,
                    type,
                    collisionObject,
                    collisionFilterGroup,
                    collisionFilterMask
                    );
            }
        }

        public void AddCollisionObject(CollisionObject collisionObject)
        {
            AddCollisionObject(collisionObject, BroadphaseProxy.CollisionFilterGroups.Default, BroadphaseProxy.CollisionFilterGroups.Default);
        }

        public void RemoveCollisionObject(CollisionObject collisionObject)
        {
            BroadphaseProxy bp = collisionObject.Broadphase;
            if (bp != null)
            {
                //
                // only clear the cached algorithms
                //
                Broadphase.CleanProxyFromPairs(bp);
                Broadphase.DestroyProxy(bp);
                collisionObject.Broadphase = null;
            }

            _collisionObjects.Remove(collisionObject);
        }

        public virtual void PerformDiscreteCollisionDetection()
        {
            DispatcherInfo dispatchInfo = DispatchInfo;
            //update aabb (of all moved objects)

            Vector3 aabbMin, aabbMax;
            for (int i = 0; i < _collisionObjects.Count; i++)
            {
                _collisionObjects[i].CollisionShape.GetAabb(_collisionObjects[i].WorldTransform, out aabbMin, out aabbMax);
                _broadphasePairCache.SetAabb(_collisionObjects[i].Broadphase, aabbMin, aabbMax);
            }

            _broadphasePairCache.RefreshOverlappingPairs();

            IDispatcher dispatcher = Dispatcher;
            if (dispatcher != null)
                dispatcher.DispatchAllCollisionPairs(_broadphasePairCache, dispatchInfo);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                //clean up remaining objects
                foreach (CollisionObject collisionObject in _collisionObjects)
                {
                    BroadphaseProxy bp = collisionObject.Broadphase;
                    if (bp != null)
                    {
                        //
                        // only clear the cached algorithms
                        //
                        Broadphase.CleanProxyFromPairs(bp);
                        Broadphase.DestroyProxy(bp);
                    }
                }
            }
        }

        /// <summary>
        /// LocalShapeInfo gives extra information for complex shapes
        /// Currently, only TriangleMeshShape is available, so it just contains triangleIndex and subpart
        /// </summary>
        [Serializable]
        public struct LocalShapeInfo
        {
            private int _shapePart;
            private int _triangleIndex;

            public int ShapePart
            {
                get { return _shapePart; }
                set { _shapePart = value; }
            }
            public int TriangleIndex
            {
                get { return _triangleIndex; }
                set { _triangleIndex = value; }
            }
        }

        [Serializable]
        public struct LocalRayResult
        {
            private CollisionObject _collisionObject;
            private LocalShapeInfo _localShapeInfo;
            private Vector3 _hitNormalLocal;
            private float _hitFraction;

            public LocalRayResult(CollisionObject collisionObject,
                LocalShapeInfo localShapeInfo,
                Vector3 hitNormalLocal,
                float hitFraction)
            {
                _collisionObject = collisionObject;
                _localShapeInfo = localShapeInfo;
                _hitNormalLocal = hitNormalLocal;
                _hitFraction = hitFraction;
            }

            public CollisionObject CollisionObject
            {
                get { return _collisionObject; }
                set { _collisionObject = value; }
            }
            public LocalShapeInfo LocalShapeInfo
            {
                get { return _localShapeInfo; }
                set { _localShapeInfo = value; }
            }
            public Vector3 HitNormalLocal
            {
                get { return _hitNormalLocal; }
                set { _hitNormalLocal = value; }
            }
            public float HitFraction
            {
                get { return _hitFraction; }
                set { _hitFraction = value; }
            }
        }

        /// <summary>
        /// RayResultCallback is used to report new raycast results
        /// </summary>
        [Serializable]
        public abstract class RayResultCallback
        {
            private float _closestHitFraction;

            public RayResultCallback()
            {
                _closestHitFraction = 1;
            }

            public float ClosestHitFraction
            {
                get { return _closestHitFraction; }
                set { _closestHitFraction = value; }
            }
            public bool HasHit
            {
                get { return _closestHitFraction < 1; }
            }

            public abstract float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace);
        }

        [Serializable]
        public class ClosestRayResultCallback : RayResultCallback
        {
            private Vector3 _rayFromWorld;//used to calculate hitPointWorld from hitFraction
            private Vector3 _rayToWorld;

            private Vector3 _hitNormalWorld;
            private Vector3 _hitPointWorld;
            private CollisionObject _collisionObject;

            public ClosestRayResultCallback(Vector3 rayFromWorld, Vector3 rayToWorld)
            {
                _rayFromWorld = rayFromWorld;
                _rayToWorld = rayToWorld;
                _collisionObject = null;
            }

            public Vector3 RayFromWorld
            {
                get { return _rayFromWorld; }
                set { _rayFromWorld = value; }
            }
            public Vector3 RayToWorld
            {
                get { return _rayToWorld; }
                set { _rayToWorld = value; }
            }
            public Vector3 HitNormalWorld
            {
                get { return _hitNormalWorld; }
                set { _hitNormalWorld = value; }
            }
            public Vector3 HitPointWorld
            {
                get { return _hitPointWorld; }
                set { _hitPointWorld = value; }
            }
            public CollisionObject CollisionObject
            {
                get { return _collisionObject; }
                set { _collisionObject = value; }
            }

            public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
            {
                //caller already does the filter on the m_closestHitFraction
                PhysDebug.Assert(rayResult.HitFraction <= ClosestHitFraction);
                ClosestHitFraction = rayResult.HitFraction;
                _collisionObject = rayResult.CollisionObject;
                if (normalInWorldSpace)
                {
                    _hitNormalWorld = rayResult.HitNormalLocal;
                }
                else
                {
                    // need to transform normal into worldspace
                    _hitNormalWorld = Vector3.Transform(rayResult.HitNormalLocal, _collisionObject.WorldTransform);
                }
                MathUtil.SetInterpolate3(_rayFromWorld, _rayToWorld, rayResult.HitFraction, ref _hitPointWorld);
                return rayResult.HitFraction;
            }
        }
    }
}
