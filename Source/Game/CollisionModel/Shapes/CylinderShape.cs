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
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Shapes
{
    /// <summary>
    /// implements cylinder shape interface
    /// </summary>
    [Serializable]
    public class CylinderShape : BoxShape
    {
        public CylinderShape(Vector3 halfExtents)
            : base(halfExtents)
        {
        }

        public override BroadphaseNativeTypes ShapeType
        {
            get
            {
                return BroadphaseNativeTypes.Cylinder;
            }
        }

        public virtual int UpAxis
        {
            get
            {
                return 1;
            }
        }

        public virtual float Radius
        {
            get
            {
                return HalfExtents.Z;
            }
        }

        //debugging
        public override string Name
        {
            get
            {
                return "CylinderY";
            }
        }

        //getAabb's default implementation is brute force, expected derived classes to implement a fast dedicated version
        public override void GetAabb(Matrix t, out Vector3 aabbMin, out Vector3 aabbMax)
        {
            //GetAabbSlow(t, out aabbMin, out aabbMax);
            base.GetAabb(t, out aabbMin, out aabbMax);
        }

        public override Vector3 LocalGetSupportingVertexWithoutMargin(Vector3 vec)
        {
            return CylinderLocalSupportY(HalfExtents, ref vec);
        }

        public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(Vector3[] vectors, Vector3[] supportVerticesOut)
        {
            Vector3 he = HalfExtents;

            for (int i = 0; i < vectors.Length; i++)
            {
                supportVerticesOut[i] = CylinderLocalSupportY(he, ref vectors[i]);
            }
        }

        public override Vector3 LocalGetSupportingVertex(Vector3 vec)
        {

            Vector3 supVertex = LocalGetSupportingVertexWithoutMargin(vec);

            if (Margin != 0)
            {
                Vector3 vecnorm = vec;

                if (vecnorm.LengthSquared() < (MathUtil.Epsilon * MathUtil.Epsilon))
                {
                    vecnorm.X = -1;
                    vecnorm.Y = -1;
                    vecnorm.Z = -1;
                }

                vecnorm.Normalize();
                supVertex += Margin * vecnorm;
            }
            return supVertex;
        }

        private Vector3 CylinderLocalSupportY(Vector3 halfExtents, ref Vector3 v)
        {
            float radius = halfExtents.X;
            float halfHeight = halfExtents.Y;//cylinderUpAxis

            Vector3 tmp = Vector3.Zero;
            float d;

            float s = (float)Math.Sqrt(v.X * v.X + v.Z * v.Z);
            if (s != 0)
            {
                d = radius / s;
                tmp.X = v.X * d;
                tmp.Y = v.Y < 0 ? -halfHeight : halfHeight;
                tmp.Z = v.Z * d;
                return tmp;
            }
            else
            {
                tmp.X = radius;
                tmp.Y = v.Y < 0 ? -halfHeight : halfHeight;
                tmp.Z = 0;
                return tmp;
            }
        }
    }
}
