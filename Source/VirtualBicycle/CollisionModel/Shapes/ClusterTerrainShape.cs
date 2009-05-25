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
using VirtualBicycle.Graphics;
using VirtualBicycle.Scene;

namespace VirtualBicycle.CollisionModel.Shapes
{
    [Serializable]
    public class ClusterTerrainShape : ConcaveShape
    {
        private Vector3 _localScaling;
        private static readonly string _objectName = "HeightfieldRes";
        private Vector3 _localAabbMin;
        private Vector3 _localAabbMax;

        //terrain data
        private float _maxHeight;


        private float[] _heightfieldDataFloat;
        //private ITriangleCallback m_callback;

        HeightField hiField;

        public ClusterTerrainShape(float cellUnit, float heightScale, GameTexture dispTexture)
        {
            _localScaling = new Vector3(cellUnit, heightScale, cellUnit);

            float quantizationMargin = 1f;

            //enlarge the AABB to avoid division by zero when initializing the quantization values
            Vector3 clampValue = new Vector3(quantizationMargin, quantizationMargin, quantizationMargin);
            Vector3 halfExtents = new Vector3(0f, 0f, 0f);

            _maxHeight = heightScale;

            halfExtents.X = Cluster.ClusterSize;
            halfExtents.Y = _maxHeight;
            halfExtents.Z = Cluster.ClusterSize;


            halfExtents *= 0.5f;

            _localAabbMin = -halfExtents - clampValue;
            _localAabbMax = halfExtents + clampValue;
            Vector3 aabbSize = _localAabbMax - _localAabbMin;

            hiField = HeightFieldManager.Instance.CreateInstance(dispTexture);
        }


        public override string Name
        {
            get { return _objectName; }
        }

        public override Vector3 LocalScaling
        {
            get { return _localScaling; }
            set { _localScaling = value; }
        }

        public override BroadphaseNativeTypes ShapeType
        {
            get { return BroadphaseNativeTypes.Terrain; }
        }

        public override void GetAabb(Matrix transform,
                                    out Vector3 aabbMin,
                                    out Vector3 aabbMax)
        {
            Vector3 halfExtents = (_localAabbMax - _localAabbMin) * _localScaling * 0.5f;

            Matrix abs_b = MathUtil.Absolute(transform);
            Vector3 center = transform.Translation;
            Vector3 row1 = new Vector3(abs_b.M11, abs_b.M12, abs_b.M13);
            Vector3 row2 = new Vector3(abs_b.M21, abs_b.M22, abs_b.M23);
            Vector3 row3 = new Vector3(abs_b.M31, abs_b.M32, abs_b.M33);
            Vector3 extent = new Vector3(Vector3.Dot(row1, halfExtents),
                                         Vector3.Dot(row2, halfExtents),
                                         Vector3.Dot(row3, halfExtents));
            extent += new Vector3(Margin, Margin, Margin);

            aabbMin = center - extent;
            aabbMax = center + extent;
        }

        private float GetHeightFieldValue(int x, int y)
        {
            return _heightfieldDataFloat[(y * Cluster.ClusterSize) + x];
        }

        private void GetVertex(int x, int y, ref Vector3 vertex)
        {
            PhysDebug.Assert(x >= 0);
            PhysDebug.Assert(y >= 0);
            PhysDebug.Assert(x < Cluster.ClusterSize);
            PhysDebug.Assert(y < Cluster.ClusterSize);

            float height = GetHeightFieldValue(x, y);

            vertex.X = (-Cluster.ClusterSize / 2) + x;
            vertex.Y = height;
            vertex.Z = (-Cluster.ClusterSize / 2) + y;

            vertex *= _localScaling;
        }

        private void QuantizeWithClamp(int[] outInt, ref Vector3 point)
        {
            Vector3 clampedPoint = new Vector3(point.X, point.Y, point.Z);

            MathUtil.SetMax(clampedPoint, _localAabbMin);
            MathUtil.SetMin(clampedPoint, _localAabbMax);

            Vector3 v = (clampedPoint);// * m_quantization;

            outInt[0] = (int)(v.X);
            outInt[1] = (int)(v.Y);
            outInt[2] = (int)(v.Z);
        }

        public override void ProcessAllTriangles(ITriangleCallback callback,
                                                Vector3 aabbMin,
                                                Vector3 aabbMax)
        {
            Vector3 localMin = new Vector3(aabbMin.X, aabbMin.Y, aabbMin.Z);
            Vector3 localMax = new Vector3(aabbMax.X, aabbMax.Y, aabbMax.Z);
            int[] quantizedAabbMin = new int[3];
            int[] quantizedAabbMax = new int[3];

            Vector3 localAabbMin = localMin *
                new Vector3(1f / _localScaling.X, 1f / _localScaling.Y, 1f / _localScaling.Z);
            Vector3 localAabbMax = localMax *
                new Vector3(1f / _localScaling.X, 1f / _localScaling.Y, 1f / _localScaling.Z);

            QuantizeWithClamp(quantizedAabbMin, ref localAabbMin);
            QuantizeWithClamp(quantizedAabbMax, ref localAabbMax);

            int startX = 0;
            int endX = Cluster.ClusterSize - 1;
            int startJ = 0;
            int endJ = Cluster.ClusterSize - 1;

            quantizedAabbMin[0] += Cluster.ClusterSize / 2 - 1;
            quantizedAabbMax[0] += Cluster.ClusterSize / 2 + 1;
            quantizedAabbMin[2] += Cluster.ClusterSize / 2 - 1;
            quantizedAabbMax[2] += Cluster.ClusterSize / 2 + 1;

            if (quantizedAabbMin[0] > startX)
                startX = quantizedAabbMin[0];
            if (quantizedAabbMax[0] < endX)
                endX = quantizedAabbMax[0];
            if (quantizedAabbMin[2] > startJ)
                startJ = quantizedAabbMin[2];
            if (quantizedAabbMax[2] < endJ)
                endJ = quantizedAabbMax[2];

            _heightfieldDataFloat = hiField.HeightData;
            if (callback != null)
            {
                Vector3[] vertices = new Vector3[3];

                for (int j = startJ; j < endJ; j++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        //first triangle
                        GetVertex(x, j, ref vertices[0]);
                        GetVertex(x, j + 1, ref vertices[1]);
                        GetVertex(x + 1, j, ref vertices[2]);

                        callback.ProcessTriangle(vertices, 0, j);

                        //second triangle
                        GetVertex(x + 1, j, ref vertices[0]);
                        GetVertex(x, j + 1, ref vertices[1]);
                        GetVertex(x + 1, j + 1, ref vertices[2]);

                        callback.ProcessTriangle(vertices, 0, j);
                    }
                }
            }
            _heightfieldDataFloat = null;
        }

        public override void CalculateLocalInertia(float mass, out Vector3 inertia)
        {
            //moving concave objects not supported
            inertia = new Vector3();
        }
    }
}
