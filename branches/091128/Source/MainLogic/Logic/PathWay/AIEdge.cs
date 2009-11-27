using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualBicycle.Logic.PathWay
{
    public enum EnumEdgeState
    {
        Open = 0,
        Closed = 1
    }

    public class AIEdge
    {
        #region Fields
        /// <summary>
        /// 获取这条边的目标端口
        /// </summary>
        private AIPort targetPort;
        public AIPort TargetPort
        {
            get { return targetPort; }
        }

        /// <summary>
        /// 获取这条边的源端口
        /// </summary>
        private AIPort sourcePort;
        public AIPort SourcePort
        {
            get { return sourcePort; }
        }

        /// <summary>
        /// 获取这条边的权值
        /// </summary>
        private float cost;
        public float Cost
        {
            get { return cost; }
        }
        
        /// <summary>
        /// 获取或设置边的状态
        /// </summary>
        private EnumEdgeState edgeState;
        public EnumEdgeState EdgeState
        {
            get { return edgeState; }
            set { edgeState = value; }
        }
        #endregion

        #region Constructor
        public AIEdge(AIPort sPort, AIPort tPort, float cost)
        {
            this.targetPort = tPort;
            this.sourcePort = sPort;
            this.cost = cost;
            edgeState = EnumEdgeState.Open;
        }

        public AIEdge(AIPort sPort, AIPort tPort, float cost,EnumEdgeState state)
        {
            this.targetPort = tPort;
            this.sourcePort = sPort;
            this.cost = cost;
            edgeState = state;
        }
        #endregion

        #region Methods

        #endregion
    }
}
