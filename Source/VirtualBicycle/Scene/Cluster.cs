using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Graphics;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.IO;
using VirtualBicycle.Logic.Traffic;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Scene
{
    /// <summary>
    ///  场景分为若干Cluster
    /// </summary>
    public class Cluster : IDisposable, IUpdatable
    {
        #region 常量

        /// <summary>
        ///  513
        /// </summary>
        public const int ClusterSize = 513;

        /// <summary>
        ///  512
        /// </summary>
        public const int ClusterLength = ClusterSize - 1;

        #endregion

        #region 字段

        protected ClusterDescription description;

        /// <summary>
        ///  保存D3D的Device对象引用
        /// </summary>
        protected Device device;

        /// <summary>
        ///  Cluster的包围球
        /// </summary>
        protected BoundingSphere boudingSphere;

        /// <summary>
        ///  这个Cluster的场景管理器
        /// </summary>
        protected OctreeSceneManager sceneMgr;

        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texSets"></param>
        /// <param name="device"></param>
        /// <param name="x">以地形单位为单位</param>
        /// <param name="y">以地形单位为单位</param>
        public Cluster(GameScene scene, Device device, int x, int y, float cellUnit)
        {
            this.description.X = x;
            this.description.Y = y;
            //this.sceneData = sceneData;
            this.device = device;
            this.CellUnit = cellUnit;

            this.boudingSphere.Center = new Vector3(
                (x + ClusterLength * 0.5f) * cellUnit,
                0,
                (y + ClusterLength * 0.5f) * cellUnit);
            this.boudingSphere.Radius = (float)ClusterSize * cellUnit * 0.5f * MathEx.Root2;

            this.WorldX = x == 0 ? 0 : (x - 1) * cellUnit;
            this.WorldZ = y == 0 ? 0 : (y - 1) * cellUnit;

            this.sceneMgr = new OctreeSceneManager(this, device, new OctreeBox(ClusterLength), 10f);
            this.GameScene = scene;
        }

        #endregion

        #region 属性

        public GameScene GameScene
        {
            get;
            private set;
        }

        /// <summary>
        ///  TODO:Commet
        /// </summary>
        public float CellUnit
        {
            get;
            protected set;
        }

        /// <summary>
        ///  TODO:Commet
        /// </summary>
        public BoundingSphere BoundingVolume
        {
            get { return boudingSphere; }
        }

        /// <summary>
        ///  TODO:Commet
        /// </summary>
        public ClusterDescription Description
        {
            get { return this.description; }
        }

        /// <summary>
        ///  TODO:Commet
        /// </summary>
        public SceneManagerBase SceneManager
        {
            get { return sceneMgr; }
        }

        /// <summary>
        ///  获取这个Cluster的在3D世界中的X坐标
        /// </summary>
        public float WorldX
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取这个Cluster的在3D世界中的Y坐标
        /// </summary>
        public float WorldY
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取这个Cluster的在3D世界中的Z坐标
        /// </summary>
        public float WorldZ
        {
            get;
            protected set;
        }

        #endregion

        #region IUpdatable 成员

        public void Update(float dt)
        {
            sceneMgr.Update(dt);
        }

        #endregion

        #region IDisposable 成员

        protected virtual void Dispose(bool disposing)
        {
            sceneMgr.Dispose();
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~Cluster()
        {
            if (!Disposed)
                Dispose(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ClusterDescription
    {
        #region 字段

        /// <summary>
        /// Cluster的X坐标（地形坐标系）。以地形单位为单位
        /// </summary>
        public int X;

        /// <summary>
        /// 以地形单位为单位
        /// </summary>
        public int Y;

        #endregion

        #region 属性

        /// <summary>
        ///  获取Cluster的宽度。
        /// </summary>
        public int Width
        {
            get { return Cluster.ClusterLength; }
        }

        /// <summary>
        ///  获取Cluster的高度。
        /// </summary>
        public int Height
        {
            get { return Cluster.ClusterLength; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">以地形单位为单位</param>
        /// <param name="y">以地形单位为单位</param>
        public ClusterDescription(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClusterDescription)
            {
                ClusterDescription v = (ClusterDescription)obj;
                return v.X == this.X && v.Y == this.Y;
            }
            return false;
        }


        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X.ToString(), Y.ToString());
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}
