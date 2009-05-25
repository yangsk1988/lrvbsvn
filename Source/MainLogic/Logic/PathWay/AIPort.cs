using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace VirtualBicycle.Logic.PathWay
{
    public class AIPort
    {
        #region Fields
        /// <summary>
        /// 获取该端口所属的道路或路口
        /// </summary>
        private AITrafficComponet owner;
        public AITrafficComponet Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// 获取该端口所属的边的信息
        /// </summary>
        private List<AIEdge> edges;
        public List<AIEdge> Edges
        {
            get { return edges; }
        }

        /// <summary>
        /// 获取或设置该端口的id
        /// </summary>
        private int uID;
        public int UID
        {
            get { return uID; }
            set { uID = value; }
        }

        /// <summary>
        /// 获取该端口的位置
        /// </summary>
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        /// <summary>
        /// 获取该端口的方向
        /// </summary>
        private Vector3 direction;
        public Vector3 Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// 获取该端口的宽度
        /// </summary>
        private float width;
        public float Width
        {
            get { return width; }
        }

        /// <summary>
        /// 获取该端口的扭曲程度
        /// </summary>
        private float twist;
        public float Twist
        {
            get { return twist; }
        }
        #endregion

        #region Constructor
        public AIPort(AITrafficComponet path, Vector3 pos, Vector3 dir, float width, float twist)
        {
            owner = path;
            this.position = pos;
            this.direction = dir;
            this.width = width;
            this.twist = twist;

            edges = new List<AIEdge>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 添加一条边的信息
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(AIEdge edge)
        {
            this.edges.Add(edge);
        }
        #endregion
    }
}
