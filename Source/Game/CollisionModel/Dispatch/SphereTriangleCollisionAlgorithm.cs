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
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Dispatch
{
    /// <summary>
    /// SphereSphereCollisionAlgorithm  provides sphere-sphere collision detection.
    /// Other features are frame-coherency (persistent data) and collision response.
    /// Also provides the most basic sample for custom/user btCollisionAlgorithm
    /// </summary>
    [Serializable]
    public class SphereTriangleCollisionAlgorithm : CollisionAlgorithm, IDisposable
    {
        private bool _ownManifold;
        private PersistentManifold _manifold;
        private bool _isSwapped;

        public SphereTriangleCollisionAlgorithm(PersistentManifold manifold, CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo, CollisionObject bodyA, CollisionObject bodyB, bool isSwapped)
            : base(collisionAlgorithmConstructionInfo)
        {
            _ownManifold = false;
            _manifold = manifold;
            _isSwapped = isSwapped;

            if (_manifold == null)
            {
                _manifold = Dispatcher.GetNewManifold(bodyA, bodyB);
                _ownManifold = true;
            }
        }

        public SphereTriangleCollisionAlgorithm(CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo)
            : base(collisionAlgorithmConstructionInfo) { }

        public override void ProcessCollision(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
        {
            if (_manifold == null)
                return;

            SphereShape sphere = bodyA.CollisionShape as SphereShape;
            TriangleShape triangle = bodyB.CollisionShape as TriangleShape;

            /// report a contact. internally this will be kept persistent, and contact reduction is done
            resultOut.SetPersistentManifold(_manifold);
            SphereTriangleDetector detector = new SphereTriangleDetector(sphere, triangle);

            DiscreteCollisionDetectorInterface.ClosestPointInput input = new DiscreteCollisionDetectorInterface.ClosestPointInput();
            input.MaximumDistanceSquared = 1e30f;//todo: tighter bounds
            input.TransformA = bodyA.WorldTransform;
            input.TransformB = bodyB.WorldTransform;

            detector.GetClosestPoints(input, resultOut, null);
        }

        public override float CalculateTimeOfImpact(CollisionObject bodyA, CollisionObject bodyB, DispatcherInfo dispatchInfo, ManifoldResult resultOut)
        {
            //not yet
            return 1f;
        }

        [Serializable]
        public class CreateFunc : CollisionAlgorithmCreateFunction
        {
            public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo collisionAlgorithmConstructionInfo, CollisionObject bodyA, CollisionObject bodyB)
            {
                return new SphereTriangleCollisionAlgorithm(collisionAlgorithmConstructionInfo.Manifold, collisionAlgorithmConstructionInfo, bodyA, bodyB, IsSwapped);
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (_ownManifold)
                if (_manifold != null)
                    Dispatcher.ReleaseManifold(_manifold);
        }
        #endregion
    }
}
