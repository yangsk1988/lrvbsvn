using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Collections;
using VirtualBicycle.Config;
using VirtualBicycle.Graphics;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  游戏中的场景
    /// </summary>
    public unsafe class GameScene : GameSceneBase<Cluster, SceneData>
    {
        ClusterTable clusterTable;

        public ClusterTable ClusterTable
        {
            get { return clusterTable; }
        }


        public GameScene(Device device, SceneData data, ProgressCallBack cbk)
            : base(device, data)
        {
            int size = (data.SceneSize - 1) / Cluster.ClusterLength;

            SceneObject[] objs = data.SceneObjects;

            int total = 2 * size * size + objs.Length;
            int current = 0;

            Cluster[] clusters = new Cluster[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    clusters[x * size + y] = new Cluster(this, device, x, y, data.CellUnit);

                    if (cbk != null)
                    {                        
                        cbk(current, total);
                        current += 2;
                    }
                }
            }

            clusterTable = new ClusterTable(clusters);


            float invCellUnit = 1.0f / CellUnit;

            for (int i = 0; i < objs.Length; i++)
            {
                int cx = (int)(objs[i].OffsetX * invCellUnit);
                int cy = (int)(objs[i].OffsetZ * invCellUnit);

                ClusterDescription desc = new ClusterDescription(cx, cy);

                Cluster cluster = clusterTable.TryGetCluster(desc);

                if (cluster != null)
                {
                    cluster.SceneManager.AddObjectToScene(objs[i]);
                }

                if (cbk != null)
                {
                    cbk(current, total);
                    current++;
                }
            }

        }

        public void GetClusterCoord(float x, float z, out int cx, out int cy)
        {
            float invCellUnit = 1.0f / CellUnit;

            int esx = (int)(x * invCellUnit);
            int esy = (int)(z * invCellUnit);

            // 截断到Cluster的第一顶点处
            if (esx < Cluster.ClusterSize)
            {
                esx = 0;
            }
            else
            {
                esx = Cluster.ClusterLength * ((esx - 1) / Cluster.ClusterLength) + 1;
            }

            if (esy < Cluster.ClusterSize)
            {
                esy = 0;
            }
            else
            {
                esy = Cluster.ClusterLength * ((esy - 1) / Cluster.ClusterLength) + 1;
            }
            cx = esx;
            cy = esy;
        }

        protected override void PrepareVisibleClusters(ICamera cam)
        {
            visibleClusters.FastClear();

            Frustum frus = cam.Frustum;
            Vector3 camPos = cam.Position;

            float invCellUnit = 1.0f / CellUnit;

            int esx = (int)((camPos.X - cam.FarPlane) * invCellUnit);
            int esy = (int)((camPos.Z - cam.FarPlane) * invCellUnit);

            // 截断到Cluster的第一顶点处
            if (esx < Cluster.ClusterSize)
            {
                esx = 0;
            }
            else
            {
                esx = Cluster.ClusterLength * ((esx - 1) / Cluster.ClusterLength) + 1;
            }

            if (esy < Cluster.ClusterSize)
            {
                esy = 0;
            }
            else
            {
                esy = Cluster.ClusterLength * ((esy - 1) / Cluster.ClusterLength) + 1;
            }


            //float a = 1.5f * cam.FarPlane / (float)Cluster.ClusterLength;
            float enumLength = cam.FarPlane * invCellUnit * 2 + Cluster.ClusterLength;
            //int enumLength = (int)a;
            //float decs = enumLength - a;

            //if (decs < -float.Epsilon)
            //{
            //    enumLength++;
            //}

            for (int x = esx; x < esx + enumLength; x += Cluster.ClusterSize)
            {
                for (int y = esy; y < esy + enumLength; y += Cluster.ClusterSize)
                {
                    ClusterDescription desc = new ClusterDescription(x, y);

                    Cluster cluster = clusterTable.TryGetCluster(desc);

                    if (cluster != null)
                    {
                        if (frus.IntersectsSphere(cluster.BoundingVolume))
                        {
                            visibleClusters.Add(cluster);
                        }
                    }
                }
            }
        }
        public override SceneObject FindObject(LineSegment ray)
        {
            Vector3 direction = ray.End - ray.Start;
            direction.Normalize();

            float cos = Vector3.Dot(Vector3.UnitY, direction);

            Vector3 insectPt = ray.Start + direction * (ray.Start.Y / cos);

            float dist = Vector3.Distance(direction, insectPt);

            float invCellUnit = 1.0f / CellUnit;
            int esx = (int)((ray.Start.X - dist) * invCellUnit);
            int esy = (int)((ray.Start.Z - dist) * invCellUnit);


            // 截断到Cluster的第一顶点处
            if (esx < Cluster.ClusterSize)
            {
                esx = 0;
            }
            else
            {
                esx = Cluster.ClusterLength * ((esx - 1) / Cluster.ClusterLength) + 1;
            }

            if (esy < Cluster.ClusterSize)
            {
                esy = 0;
            }
            else
            {
                esy = Cluster.ClusterLength * ((esy - 1) / Cluster.ClusterLength) + 1;
            }
            float enumLength = dist * invCellUnit * 2 + Cluster.ClusterLength;

            float minDist = float.MaxValue;
            SceneObject result = null;

            Ray ra = new Ray(ray.Start, direction);

            for (int x = esx; x < esx + enumLength; x += Cluster.ClusterSize)
            {
                for (int y = esy; y < esy + enumLength; y += Cluster.ClusterSize)
                {
                    ClusterDescription desc = new ClusterDescription(x, y);

                    Cluster cluster = clusterTable.TryGetCluster(desc);

                    if (cluster != null)
                    {
                        SceneObject sceObj = cluster.SceneManager.FindObject(ra);
                        if (sceObj != null)
                        {
                            Vector3 pos = sceObj.BoundingSphere.Center;
                            pos.X += sceObj.OffsetX;
                            pos.Y += sceObj.OffsetY;
                            pos.Z += sceObj.OffsetZ;

                            float curDist = Vector3.Distance(pos, ray.Start);

                            if (curDist < minDist)
                            {
                                minDist = curDist;
                                result = sceObj;
                            }
                        }
                    }
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing) 
            {
                foreach (Cluster c in clusterTable)
                {
                    c.Dispose();
                }
            }
            clusterTable = null;
        }

    }
}
