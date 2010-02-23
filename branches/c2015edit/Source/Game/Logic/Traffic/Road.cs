using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using VirtualBicycle.Collections;
using VirtualBicycle.CollisionModel;
using VirtualBicycle.Graphics;
using VirtualBicycle.Graphics.Effects;
using VirtualBicycle.IO;
using VirtualBicycle.MathLib;
using VirtualBicycle.Physics;
using VirtualBicycle.Physics.Dynamics;
using VirtualBicycle.Scene;
using System.ComponentModel;

namespace VirtualBicycle.Logic.Traffic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="LType"></typeparam>
    public class Road : SceneObject, ITrafficComponment
    {
        #region 常量
        static readonly string IdTag = "Id";
        static readonly string ConnectorTag = "Connector";


        static readonly string ConnectionTag = "Connection";
        static readonly string ConnectionCountTag = "ConnectionCount";

        static readonly string OffsetTag = "Offset";

        static readonly string SegmentTag = "Segment";
        protected static readonly string SegmentCountTag = "SegmentCount";
        protected static readonly string TextureNameTag = "TextureName";

        public static readonly string DefaultTextureName = "RoadDefault";

        #endregion

        #region 字段

        RoadLine trackLine;

        Device device;

        protected Model[] models;

        RoadSegment[] bufferedTracks;
        TCPort[] connectors;
        ExistTable<ITrafficComponment> connectTable;

        List<TCConnectionInfo> connectedCon;
        TrafficNet trafficNet;
        #endregion

        #region 构造函数
        public Road(Device device, TrafficNet trafficNet)
            : this(device)
        {
            this.trafficNet = trafficNet;

            this.connectTable = new ExistTable<ITrafficComponment>();
            this.connectedCon = new List<TCConnectionInfo>();
        }

        public Road(Device device, RoadLine line, RoadBuilder trackBuilder, TrafficNet trafficNet)
            : this(device)
        {
            this.models = trackBuilder.BuildModels(device, line);
            this.trafficNet = trafficNet;
            this.connectTable = new ExistTable<ITrafficComponment>();
            this.connectedCon = new List<TCConnectionInfo>();
            this.TrackLine = line;

            for (int i = 0; i < SegmentCount; i++)
            {
                CacheMemory mem = Cache.Instance.Allocate();
                models[i].SetCache(mem);

            }
            this.TextureName = DefaultTextureName;

            BuildConnectors(line);
        }

        void BuildConnectors(RoadLine line)
        {
            RoadNode[] nodes = line.InputPoints;
            int a = nodes.Length - 2;

            connectors = new TCPort[2];

            connectors[0].Position = nodes[1].Position;
            connectors[0].Width = nodes[1].Width;
            connectors[0].Twist = nodes[1].Twist;

            connectors[1].Position = nodes[a].Position;
            connectors[1].Width = nodes[a].Width;
            connectors[1].Twist = nodes[a].Twist;
        }

        private Road(Device device)
            : base(false)
        {
            this.device = device;
            this.textureName = DefaultTextureName;

            this.BoundingSphere.Radius = 0.5f * float.MaxValue;
        }

        #endregion

        #region 属性

        [Browsable(false)]
        public object Tag
        {
            get;
            set;
        }
        
        int id;

        bool hasId;

        [Browsable(false)]
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

        public void SetId(int id)
        {
            Id = id;
        }

        //public string Name
        //{
        //    get;
        //    protected set;
        //}

        string textureName;
        public string TextureName
        {
            get { return textureName; }
            set
            {
                textureName = value;

                if (models != null)
                {
                    for (int i = 0; i < SegmentCount; i++)
                    {
                        for (int j = 0; j < models[i].Entities.Length; j++)
                        {
                            MeshMaterial[][] mats = models[i].Entities[j].Materials;

                            for (int a = 0; a < mats.Length; a++)
                            {
                                for (int b = 0; b < mats[a].Length; b++)
                                {
                                    mats[a][b].SetTexture(0, TextureLibrary.Instance.GetColorMap(TextureName));
                                    mats[a][b].SetTexture(1, TextureLibrary.Instance.GetNormalMap(TextureName));
                                    mats[a][b].SetEffect(EffectManager.Instance.GetModelEffect(RoadEffectFactory.Name));

                                    mats[a][b].Ambient = new Color4(0.35f, 0.35f, 0.35f);
                                    mats[a][b].Diffuse = new Color4(0.85f, 0.85f, 0.85f);
                                    mats[a][b].Specular = new Color4(0.9f, 0.9f, 0.9f);
                                    mats[a][b].Power = 16;
                                }
                            }
                        }
                    }
                }
            }
        }

        public RoadLine TrackLine
        {
            get { return trackLine; }
            protected set { trackLine = value; }
        }

        public int SegmentCount
        {
            get
            {
                if (models == null)
                    return 0;
                return models.Length;
            }
        }

        #endregion

        public override void BuildPhysicsModel(DynamicsWorld world)
        {
            RoadSegment[] tracks = GetTracks();
            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i].BuildPhysicsModel(world);
            }
        }

        #region 方法

        public Model[] GetModels()
        {
            return models;
        }

        public RoadSegment[] GetTracks()
        {
            if (bufferedTracks == null)
            {
                bufferedTracks = new RoadSegment[SegmentCount];

                for (int i = 0; i < SegmentCount; i++)
                {
                    bufferedTracks[i] = new RoadSegment(this, models[i]);
                    bufferedTracks[i].BoundingSphere.Radius = float.MaxValue * 0.5f;
                }
            }
            return bufferedTracks;
        }

        public override void OnAddedToScene(object sender, OctreeSceneManager sceneMgr)
        {
            RoadSegment[] trackSeg = GetTracks();

            for (int i = 0; i < trackSeg.Length; i++)
            {
                sceneMgr.AddObjectToScene(trackSeg[i]);
            }
            trafficNet.NotifyComponmentAdded(this);
        }

        public override void OnRemovedFromScene(object sender, OctreeSceneManager sceneMgr)
        {
            RoadSegment[] trackSeg = bufferedTracks;

            if (trackSeg != null)
            {
                for (int i = 0; i < trackSeg.Length; i++)
                {
                    sceneMgr.RemoveObjectFromScene(trackSeg[i]);
                }
            }
            trafficNet.NotifyComponmentRemoved(this);
        }

        #endregion

        #region SceneObject 序列化

        public override bool IsSerializable
        {
            get { return true; }
        }

        public override string TypeTag
        {
            get { return RoadFactory.TypeId; }
        }

        public override void Serialize(BinaryDataWriter data)
        {
            trackLine.WriteData(data);

            data.AddEntry(SegmentCountTag, SegmentCount);

            ContentBinaryWriter bw = data.AddEntry(TextureNameTag);
            bw.WriteStringUnicode(TextureName);
            bw.Close();

            for (int i = 0; i < SegmentCount; i++)
            {
                VirtualStream stream = new VirtualStream(data.AddEntryStream(SegmentTag + i.ToString()));
                Model.ToStream(models[i], stream);
            }

            bw = data.AddEntry(OffsetTag);
            bw.Write(OffsetX);
            bw.Write(OffsetY);
            bw.Write(OffsetZ);
            bw.Close();


            data.AddEntry(IdTag, Id);

            bw = data.AddEntry(ConnectorTag);

            for (int i = 0; i < 2; i++)
            {
                bw.Write(connectors[i].Position.X);
                bw.Write(connectors[i].Position.Y);
                bw.Write(connectors[i].Position.Z);
                bw.Write(connectors[i].Width);
                bw.Write(connectors[i].Twist);
            }

            bw.Close();


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

        public virtual void Deserialize(BinaryDataReader data)
        {
            int segmentCount = data.GetDataInt32(SegmentCountTag);

            trackLine = new RoadLine();

            trackLine.ReadData(data);

            models = new Model[segmentCount];
            for (int i = 0; i < segmentCount; i++)
            {
                StreamedLocation sl = new StreamedLocation(data.GetDataStream(SegmentTag + i.ToString()));

                models[i] = ModelManager.Instance.CreateInstance(device, sl);
            }

            ContentBinaryReader br = data.GetData(OffsetTag);
            OffsetX = br.ReadSingle();
            OffsetY = br.ReadSingle();
            OffsetZ = br.ReadSingle();
            br.Close();


            br = data.GetData(TextureNameTag);
            TextureName = br.ReadStringUnicode();
            br.Close();

            if (data.Contains(IdTag))
            {
                Id = data.GetDataInt32(IdTag);
            }

            this.connectors = new TCPort[2];

            br = data.TryGetData(ConnectorTag);
            if (br != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    connectors[i].Position.X = br.ReadSingle();
                    connectors[i].Position.Y = br.ReadSingle();
                    connectors[i].Position.Z = br.ReadSingle();
                    connectors[i].Width = br.ReadSingle();
                    connectors[i].Twist = br.ReadSingle();
                }
                br.Close();
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

        public void SetConnections(List<TCConnectionInfo> info)
        {
            connectedCon = info;
            if (connectTable != null)
            {
                connectTable.Clear();
            }
            else
            {
                connectTable = new ExistTable<ITrafficComponment>(info.Count);
            }

            for (int i = 0; i < info.Count; i++) 
            {
                connectTable.Add(info[i].TargetComponet);
            }
        }


        public bool IsConnected(ITrafficComponment con)
        {
            return connectTable.Exists(con);
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

        public override void Update(float dt)
        {

        }
        public override RenderOperation[] GetRenderOperation()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (bufferedTracks != null)
                {
                    for (int i = 0; i < bufferedTracks.Length; i++)
                    {
                        if (!bufferedTracks[i].Disposed)
                            bufferedTracks[i].Dispose();
                    }
                }
                if (models != null)
                {
                    for (int i = 0; i < SegmentCount; i++)
                    {
                        for (int j = 0; j < models[i].Entities.Length; j++)
                        {
                            MeshMaterial[][] mats = models[i].Entities[j].Materials;

                            for (int a = 0; a < mats.Length; a++)
                            {
                                for (int b = 0; b < mats[a].Length; b++)
                                {
                                    mats[a][b].SetTexture(0, null);
                                    mats[a][b].SetTexture(1, null);
                                }
                            }
                        }
                        if (!models[i].Disposed)
                            ModelManager.Instance.DestoryInstance(models[i]);
                    }
                }
            }
            bufferedTracks = null;
        }
    }
}
