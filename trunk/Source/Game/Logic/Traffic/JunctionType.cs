using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using VirtualBicycle.Config;
using VirtualBicycle.MathLib;

namespace VirtualBicycle.Logic.Traffic
{
    public class JunctionType : EntityType
    {
        struct PortEntry
        {
            public Vector3 Position;
            public Vector3 Direction;
            public float Width;
            public float Twist;

            public PortEntry(Vector3 pos,Vector3 dir, float width, float twist)
            {
                this.Position = pos;
                this.Direction = dir;
                this.Width = width;
                this.Twist = twist;
            }
        }

        List<PortEntry> ports = new List<PortEntry>();


        #region 属性

        public int PortCount
        {
            get { return ports.Count; }
        }

        #endregion

        #region 方法

        public Junction CreateInstance(TrafficNet trafficNet)
        {
            Junction result = new Junction(this, trafficNet);

            return result;
        }

        public Vector3 GetPortDirection(int index)
        {
            return ports[index].Direction;
        }
        public Vector3 GetPortCoord(int index)
        {
            return ports[index].Position;
        }

        public float GetPortWidth(int index)
        {
            return ports[index].Width;
        }

        public float GetPortTwist(int index)
        {
            return ports[index].Twist;
        }

        #endregion

        #region IConfigurable 成员

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            ports.Clear();

            int portCount = sect.GetInt("PortCount");

            ports.Capacity = portCount;

            for (int i = 0; i < portCount; i++)
            {
                PortEntry ent;

                string name = "Port" + (i + 1).ToString();

                float[] pos = sect.GetSingleArray(name + "Coord");
                float[] dir = sect.GetSingleArray(name + "Dir");
                float width = sect.GetSingle(name + "Width");
                float twist = MathEx.Degree2Radian(sect.GetSingle(name + "Twist", 0));

                ent.Position.X = pos[0];
                ent.Position.Y = pos[1];
                ent.Position.Z = pos[2];

                ent.Direction.X = dir[0];
                ent.Direction.Y = dir[1];
                ent.Direction.Z = dir[2];

                ent.Width = width;
                ent.Twist = twist;

                ports.Add(ent);
            }
        }


        #endregion
    }
}
