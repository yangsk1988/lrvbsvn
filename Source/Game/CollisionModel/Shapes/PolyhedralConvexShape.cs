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
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.MathLib;

namespace VirtualBicycle.CollisionModel.Shapes
{
#pragma warning disable 
    [Serializable]
    public abstract class PolyhedralConvexShape : ConvexShape
    {
        private Vector3 _localAabbMin;
        private Vector3 _localAabbMax;
        private bool _isLocalAabbValid;

        public PolyhedralConvexShape()
        {
            _localAabbMin = new Vector3(1, 1, 1);
            _localAabbMax = new Vector3(-1, -1, -1);
        }

        public abstract int VertexCount { get; }
        public abstract int EdgeCount { get; }
        public abstract int PlaneCount { get; }

        public abstract void GetEdge(int i, out Vector3 pointA, out Vector3 pointB);
        public abstract void GetVertex(int i, out Vector3 vertex);
        public abstract void GetPlane(out Vector3 planeNormal, out Vector3 planeSupport, int i);
        //	abstract int getIndex(int i); 

        public abstract bool IsInside(Vector3 point, float tolerance);

        // optional Hull is for optional Separating Axis Test Hull collision detection, see Hull.cpp
        //public class Hull m_optionalHull;

        public override Vector3 LocalGetSupportingVertexWithoutMargin(Vector3 vec)
        {
            Vector3 supVec = new Vector3();

            float maxDot = -1e30f;

            float lenSqr = vec.LengthSquared();
            if (lenSqr < 0.0001f)
            {
                vec = new Vector3(1, 0, 0);
            }
            else
            {
                float rlen = 1f / (float)Math.Sqrt(lenSqr);
                vec *= rlen;
            }

            Vector3 vtx;
            float newDot;

            for (int i = 0; i < VertexCount; i++)
            {
                GetVertex(i, out vtx);
                newDot = Vector3.Dot(vec, vtx);
                if (newDot > maxDot)
                {
                    maxDot = newDot;
                    supVec = vtx;
                }
            }
            return supVec;
        }

        public override void BatchedUnitVectorGetSupportingVertexWithoutMargin(Vector3[] vectors, Vector3[] supportVerticesOut)
        {
#warning Think about this
            /*Vector3 vtx;
			float newDot;

			for (int i = 0; i < vectors.Length; i++)
			{
				supportVerticesOut[i][3] = -1e30f;
			}

			for (int j = 0; j < vectors.Length; j++)
			{
				Vector3 vec = vectors[j];

				for (int i = 0; i < getNumVertices(); i++)
				{
					getVertex(i, out vtx);
					newDot = Vector3.Dot(vec,vtx);
					if (newDot > supportVerticesOut[j][3])
					{
						//WARNING: don't swap next lines, the w component would get overwritten!
						supportVerticesOut[j] = vtx;
						supportVerticesOut[j][3] = newDot;
					}
				}
			}*/
        }

        public override void CalculateLocalInertia(float mass, out Vector3 inertia)
        {
            //not yet, return box inertia
            float margin = Margin;

            Matrix ident = Matrix.Identity;
            Vector3 aabbMin, aabbMax;
            GetAabb(ident, out aabbMin, out aabbMax);
            Vector3 halfExtents = (aabbMax - aabbMin) * 0.5f;

            float lx = 2f * (halfExtents.X + margin);
            float ly = 2f * (halfExtents.Y + margin);
            float lz = 2f * (halfExtents.Z + margin);
            float x2 = lx * lx;
            float y2 = ly * ly;
            float z2 = lz * lz;
            float scaledmass = mass * 0.08333333f;

            inertia = scaledmass * (new Vector3(y2 + z2, x2 + z2, x2 + y2));
        }

        public override void GetAabb(Matrix trans, out Vector3 aabbMin, out Vector3 aabbMax)
        {
            //lazy evaluation of local aabb
            PhysDebug.Assert(_isLocalAabbValid);

            PhysDebug.Assert(_localAabbMin.X <= _localAabbMax.X);
            PhysDebug.Assert(_localAabbMin.Y <= _localAabbMax.Y);
            PhysDebug.Assert(_localAabbMin.Z <= _localAabbMax.Z);


            Vector3 localHalfExtents = 0.5f * (_localAabbMax - _localAabbMin);
            Vector3 localCenter = 0.5f * (_localAabbMax + _localAabbMin);

            Matrix abs_b = MathUtil.Absolute(trans);

            Vector3 center = MathUtil.MatrixToVector(trans, localCenter);

            Vector3 extent = new Vector3(
                Vector3.Dot(MathUtil.GetRow(abs_b, 0), localHalfExtents),
                Vector3.Dot(MathUtil.GetRow(abs_b, 1), localHalfExtents),
                Vector3.Dot(MathUtil.GetRow(abs_b, 2), localHalfExtents)
                                        );

            extent += new Vector3(Margin, Margin, Margin);

            aabbMin = center - extent;
            aabbMax = center + extent;
        }

        public void RecalcLocalAabb()
        {
            _isLocalAabbValid = true;

            for (int i = 0; i < 3; i++)
            {
                Vector3 vec = new Vector3();
                MathUtil.SetValueByIndex(ref vec, i, 1);
                Vector3 tmp = LocalGetSupportingVertex(vec);
                MathUtil.SetValueByIndex(ref _localAabbMax, i, MathUtil.GetValueByIndex(tmp, i) + CollisionMargin);
                MathUtil.SetValueByIndex(ref vec, i, -1);
                tmp = LocalGetSupportingVertex(vec);
                MathUtil.SetValueByIndex(ref _localAabbMin, i, MathUtil.GetValueByIndex(tmp, i) - CollisionMargin);
            }
        }
    }
}
