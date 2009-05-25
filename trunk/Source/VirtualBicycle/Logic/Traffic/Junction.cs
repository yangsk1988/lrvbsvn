using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.IO;
using VirtualBicycle.Scene;
using VirtualBicycle.Collections;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Logic.Traffic
{
    public class Junction : StaticObject, ITrafficComponment
    {
        static readonly string OrientationTag = "Orientation";
        static readonly string PositionTag = "Position";
        static readonly string OffsetTag = "Offset";
        static readonly string ConnectionTag = "Connection";
        static readonly string ConnectionCountTag = "ConnectionCount";
        static readonly string IdTag = "Id";



        const int RoadIdNone = -1;

        JunctionType crossType;


        TCPort[] connectors_o;
        TCPort[] connectors;
        ExistTable<ITrafficComponment> connectTable;

        List<TCConnectionInfo> connectedCon;
        TrafficNet trafficNet;

        bool hasId;

        #region 属性

        public override bool IsSerializable
        {
            get { return true; }
        }

        public string TypeName
        {
            get { return crossType.TypeName; }
        }


        #endregion

        public Junction(JunctionType type, TrafficNet trafficNet)
        {
            this.crossType = type;
            this.trafficNet = trafficNet;

            this.Model = type.Model;
            this.LodModel = type.LodModel;

            this.connectTable = new ExistTable<ITrafficComponment>();
            this.connectedCon = new List<TCConnectionInfo>();

            int portCount = type.PortCount;
            this.connectors = new TCPort[portCount];
            this.connectors_o = new TCPort[portCount];

            for (int i = 0; i < portCount; i++)
            {
                connectors[i].Position = type.GetPortCoord(i);
                connectors[i].Direction = type.GetPortDirection(i);
                connectors[i].Width = type.GetPortWidth(i);
                connectors[i].Twist = type.GetPortTwist(i);
                
                connectors_o[i].Position = connectors[i].Position;
                connectors_o[i].Direction = connectors[i].Direction;
                connectors_o[i].Width = connectors[i].Width;
                connectors_o[i].Twist = connectors[i].Twist;
            }

        }

        #region 方法

        public void LoadData(BinaryDataReader data)
        {
            ContentBinaryReader br = data.GetData(PositionTag);
            Vector3 pos;
            pos.X = br.ReadSingle();
            pos.Y = br.ReadSingle();
            pos.Z = br.ReadSingle();
            br.Close();

            br = data.GetData(OrientationTag);
            Quaternion quat;
            quat.W = br.ReadSingle();
            quat.X = br.ReadSingle();
            quat.Y = br.ReadSingle();
            quat.Z = br.ReadSingle();
            br.Close();

            Position = pos;
            Orientation = quat;

            br = data.GetData(OffsetTag);
            OffsetX = br.ReadSingle();
            OffsetY = br.ReadSingle();
            OffsetZ = br.ReadSingle();
            br.Close();


            if (data.Contains(IdTag))
            {
                Id = data.GetDataInt32(IdTag);
            }

            int roadCount = data.GetDataInt32(ConnectionCountTag, 0);
            connectedCon.Capacity = roadCount;

            for (int i = 0; i < roadCount; i++)
            {
                br = data.GetData(ConnectionTag + i.ToString());

                connectedCon.Add(new TCConnectionInfo(br.ReadInt32(), this, br.ReadInt32(), br.ReadInt32()));

                br.Close();
            }
        }

        public void SaveData(BinaryDataWriter data)
        {
            ContentBinaryWriter bw = data.AddEntry(PositionTag);
            Vector3 pos = Position;
            bw.Write(pos.X);
            bw.Write(pos.Y);
            bw.Write(pos.Z);
            bw.Close();


            bw = data.AddEntry(OrientationTag);
            Quaternion quat = Orientation;
            bw.Write(quat.W);
            bw.Write(quat.X);
            bw.Write(quat.Y);
            bw.Write(quat.Z);
            bw.Close();


            bw = data.AddEntry(OffsetTag);
            bw.Write(OffsetX);
            bw.Write(OffsetY);
            bw.Write(OffsetZ);
            bw.Close();

            data.AddEntry(IdTag, Id);


            data.AddEntry(ConnectionCountTag, connectedCon.Count);

            for (int i = 0; i < connectedCon.Count; i++)
            {
                bw = data.AddEntry(ConnectionTag + i.ToString());

                bw.Write(connectedCon[i].SelfPortIndex);
                bw.Write(connectedCon[i].TargetPortIndex);
                bw.Write(connectedCon[i].TargetComponmentId);


                bw.Close();
            }
        }

        public override void Serialize(BinaryDataWriter data)
        {
            JunctionFactory.Serialize(this, data);
        }

        public override void OnAddedToScene(object sender, OctreeSceneManager sceneMgr)
        {
            base.OnAddedToScene(sender, sceneMgr);
            trafficNet.NotifyComponmentAdded(this);
        }

        public override void OnRemovedFromScene(object sender, OctreeSceneManager sceneMgr)
        {
            base.OnRemovedFromScene(sender, sceneMgr);
            trafficNet.NotifyComponmentRemoved(this);
        }
        public override void UpdateTransform()
        {
            base.UpdateTransform();

            for (int i = 0; i < connectors.Length; i++)
            {
                connectors[i].Position = connectors_o[i].Position;
                connectors[i].Direction = connectors_o[i].Direction;

                MathEx.MatrixTransformVec(ref Transformation, ref connectors[i].Direction);
                MathEx.MatrixTransformPoint(ref Transformation, ref connectors[i].Position);

                connectors[i].Direction.Normalize();

            }
        }
        #endregion

        #region SceneObject 序列化

        public override string TypeTag
        {
            get { return JunctionFactory.TypeId; }
        }
        #endregion

        #region IRoadConponment 成员

        public TCPort[] GetPorts()
        {
            return connectors;
        }

        public List<TCConnectionInfo> GetConnections()
        {
            return connectedCon;
        }

        int id;

        public int Id
        {
            get
            {
                if (hasId)
                    return id;

                id = trafficNet.GenerateId(this);
                hasId = true;
                return id;
            }
            private set
            {
                id = value;
                hasId = true;
            }
        }

        public void ConnectWithPasv(ITrafficComponment con, int selfPort, int targetPort)
        {
            if (!object.ReferenceEquals(con, this))
            {
                TCConnectionInfo info;
                info.TargetComponet = con;
                info.TargetComponmentId = con.Id;
                info.SelfPortIndex = selfPort;
                info.SelfComponet = this;
                info.TargetPortIndex = targetPort;

                connectedCon.Add(info);

                connectTable.Add(con);
            }
        }
        public void ConnectWith(ITrafficComponment con, int selfPort, int targetPort)
        {
            if (!object.ReferenceEquals(con, this))
            {
                TCConnectionInfo info;
                info.TargetComponet = con;
                info.TargetComponmentId = con.Id;
                info.SelfPortIndex = selfPort;
                info.SelfComponet = this;
                info.TargetPortIndex = targetPort;

                connectedCon.Add(info);

                connectTable.Add(con);

                con.ConnectWithPasv(this, targetPort, selfPort);
            }
        }
        public void DisconnectWith(ITrafficComponment con)
        {

        }

        public bool IsConnected(ITrafficComponment con)
        {
            return connectTable.Exists(con);
        }

        bool isParsed;

        public void ParseNodes() 
        {
            if (!isParsed)
            {
                for (int i = 0; i < connectedCon.Count; i++)
                {
                    TCConnectionInfo info = connectedCon[i];
                    info.TargetComponet = trafficNet.GetComponment(info.TargetComponmentId);

                    connectedCon[i] = info;

                    connectTable.Add(info.TargetComponet);
                }

                isParsed = true;
            }
            else
            {
                throw new Exception();
            }
        }

        #endregion
    }
}
