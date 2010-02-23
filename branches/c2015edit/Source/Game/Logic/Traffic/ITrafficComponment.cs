using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace VirtualBicycle.Logic.Traffic
{
    public struct TCPort
    {
        public Vector3 Position;
        public Vector3 Direction;

        public float Width;
        public float Twist;
    }

    public struct TCConnectionInfo
    {
        public int SelfPortIndex;
        public ITrafficComponment SelfComponet;
        public int TargetPortIndex;
        public int TargetComponmentId;
        public ITrafficComponment TargetComponet;

        public TCConnectionInfo(int selfport, ITrafficComponment selfComponet, int targetPort, int targetComponetID)
        {
            this.SelfPortIndex = selfport;
            this.SelfComponet = selfComponet;
            this.TargetPortIndex = targetPort;
            this.TargetComponmentId = targetComponetID;
            this.TargetComponet = null;
        }
    }

    public interface ITrafficComponment
    {
        /// <summary>
        ///  获取这个道路的连接口
        /// </summary>
        /// <returns></returns>
        TCPort[] GetPorts();

        List<TCConnectionInfo> GetConnections();

        bool IsConnected(ITrafficComponment con);

        void ConnectWith(ITrafficComponment con, int selfPort, int targetPort);
        void ConnectWithPasv(ITrafficComponment con, int selfPort, int targetPort);
        void DisconnectWith(ITrafficComponment con);

        void ParseNodes();

        int Id
        {
            get;
        }
    }
}
